import { useState } from 'react';
import * as FileSystem from 'expo-file-system';

interface UploadProgress {
  totalBytes: number;
  sentBytes: number;
  percent: number;
}

interface UseFileUploadReturn {
  upload: (uri: string, fileName: string) => Promise<string>;
  isUploading: boolean;
  progress: UploadProgress | null;
  uploadError: Error | null;
  resetState: () => void;
}

/**
 * A hook for uploading files to Azure Blob Storage (mock implementation)
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

  // Mock implementation that simulates uploading a file to Azure Blob Storage
  const upload = async (uri: string, fileName: string): Promise<string> => {
    try {
      setIsUploading(true);
      setUploadError(null);

      // Get file info
      const fileInfo = await FileSystem.getInfoAsync(uri);

      if (!fileInfo.exists) {
        throw new Error(`File does not exist: ${uri}`);
      }

      // Simulate upload progress with a delay
      const totalBytes = fileInfo.size ?? 0;
      const progressInterval = setInterval(() => {
        setProgress((prev) => {
          if (!prev) return { totalBytes, sentBytes: 0, percent: 0 };

          // Simulate 20% progress increase every interval
          const newPercent = Math.min(prev.percent + 20, 100);
          const newSentBytes = Math.floor((newPercent / 100) * totalBytes);

          return {
            totalBytes,
            sentBytes: newSentBytes,
            percent: newPercent,
          };
        });
      }, 500);

      // Mock upload delay - would be a real Azure SDK call in production
      await new Promise((resolve) => setTimeout(resolve, 2500));

      clearInterval(progressInterval);

      setProgress({
        totalBytes,
        sentBytes: totalBytes,
        percent: 100,
      });

      // Return a mock Azure Blob Storage URL
      const blobUrl = `https://mockstorageaccount.blob.core.windows.net/audio/${fileName}`;
      setIsUploading(false);
      return blobUrl;
    } catch (error) {
      setIsUploading(false);
      const err = error instanceof Error ? error : new Error('Unknown upload error');
      setUploadError(err);
      throw err;
    }
  };

  return { upload, isUploading, progress, uploadError, resetState };
}
