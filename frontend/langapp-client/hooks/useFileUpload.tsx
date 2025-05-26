import { useState } from 'react';
import * as FileSystem from 'expo-file-system';
import { getUploadSasUri as getCustomUploadSasUri } from '@/api/functions/orval/file-upload';
import {
  GetUploadSasUriParams,
  ResponseModel,
} from '@/api/functions/orval/openAPIDocumentOnAzureFunctions.schemas';
import { functionsApiMutator } from '@/api/axiosMutator';
import { escapeNonAscii } from '@/lib/utils';

interface UploadProgress {
  totalBytes: number;
  sentBytes: number;
  percent: number;
}

interface UseFileUploadReturn {
  upload: (uri: string, fileName: string, mimeType?: string) => Promise<string>;
  isUploading: boolean;
  progress: UploadProgress | null;
  uploadError: Error | null;
  resetState: () => void;
}

/**
 * A hook for uploading files to Azure Blob Storage
 */
export function useFileUpload(): UseFileUploadReturn {
  const [isUploading, setIsUploading] = useState(false);
  const [progress, setProgress] = useState<UploadProgress | null>(null);
  const [uploadError, setUploadError] = useState<Error | null>(null);

  const resetState = () => {
    setIsUploading(false);
    setProgress(null);
    setUploadError(null);
  };

  /**
   * Upload a file to Azure Blob Storage
   * @param uri Local file URI to upload
   * @param fileName Filename to use in blob storage
   * @param mimeType MIME type of the file (default: 'application/octet-stream')
   * @returns URL of the uploaded file
   */
  const upload = async (uri: string, fileName: string, mimeType?: string): Promise<string> => {
    try {
      setIsUploading(true);
      setUploadError(null);

      // Get file info
      const fileInfo = await FileSystem.getInfoAsync(uri);

      if (!fileInfo.exists) {
        throw new Error(`File does not exist: ${uri}`);
      }

      // 1. Get an upload SAS URI from the API
      const uploadResponse = await getCustomUploadSasUri({ fileName });

      if (!uploadResponse?.uploadUri) {
        throw new Error('Failed to obtain upload URL');
      }

      const { uploadUri } = uploadResponse; // blobFileName from response can be useful if API changes it
      const totalBytes = fileInfo.size ?? 0;

      // Set initial progress
      setProgress({
        totalBytes,
        sentBytes: 0,
        percent: 0,
      });

      // Track progress manually since Expo FileSystem's progress tracking is platform-specific
      const progressTracker = setInterval(() => {
        setProgress((prev) => {
          if (!prev) return { totalBytes, sentBytes: 0, percent: 0 };
          // Don't go to 100% until confirmed completion
          const newPercent = Math.min(prev.percent + 10, 95);
          const newSentBytes = Math.floor((newPercent / 100) * totalBytes);
          return {
            totalBytes,
            sentBytes: newSentBytes,
            percent: newPercent,
          };
        });
      }, 300);

      try {
        console.log('Starting upload to:', uploadUri);
        const uploadResult = await FileSystem.uploadAsync(uploadUri, uri, {
          httpMethod: 'PUT',
          uploadType: FileSystem.FileSystemUploadType.BINARY_CONTENT,
          headers: {
            'Content-Type': mimeType || 'application/octet-stream',
            'x-ms-blob-type': 'BlockBlob',

            'x-ms-meta-originalfilename': escapeNonAscii(fileName),
          },
        });

        if (uploadResult.status !== 200 && uploadResult.status !== 201) {
          throw new Error(`Upload failed with status ${uploadResult.status}`);
        }
      } finally {
        console.log('Upload completed:', uploadUri);
        clearInterval(progressTracker);
      }

      // Upload successful, set final progress
      setProgress({
        totalBytes,
        sentBytes: totalBytes,
        percent: 100,
      });

      setIsUploading(false);

      const cleanBlobUrl = uploadUri.split('?')[0];
      return cleanBlobUrl;
    } catch (error) {
      console.error('File upload error:', JSON.stringify(error.response, null, 2));
      setIsUploading(false);
      const err = error instanceof Error ? error : new Error('Unknown upload error');
      setUploadError(err);
      throw err;
    }
  };

  return { upload, isUploading, progress, uploadError, resetState };
}
