import { useState } from 'react';
import { getReadSasUri } from '@/api/functions/orval/file-access';

interface UseFileAccessReturn {
  getReadUrl: (blobUrl: string) => Promise<string>;
  isLoading: boolean;
  error: Error | null;
}

/**
 * A hook for retrieving read access URLs for files in Azure Blob Storage
 */
export function useFileAccess(): UseFileAccessReturn {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  /**
   * Get a read access URL for a blob in storage
   * @param blobUrl The direct URL to the blob, e.g., https://<account>.blob.core.windows.net/<container>/<blobName>
   * @returns A URL that can be used to read the blob (with SAS token if necessary for private blobs)
   */
  const getReadUrl = async (blobUrl: string): Promise<string> => {
    try {
      setIsLoading(true);
      setError(null);

      const urlParts = new URL(blobUrl);
      const pathSegments = urlParts.pathname.split('/').filter(Boolean);

      if (pathSegments.length < 2) {
        throw new Error(
          'Invalid blob URL format. Expected format: https://<account>.blob.core.windows.net/<container>/<blobName>'
        );
      }

      const containerName = pathSegments[0];
      const blobFileName = pathSegments.slice(1).join('/');

      // Get a SAS token for the blob
      const readResponse = await getReadSasUri({
        containerName,
        blobFileName,
      });

      if (!readResponse?.readUri) {
        throw new Error('Failed to obtain read URL with SAS token');
      }

      return readResponse.readUri;
    } catch (error) {
      const err = error instanceof Error ? error : new Error('Unknown error getting file URL');
      setError(err);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  return { getReadUrl, isLoading, error };
}
