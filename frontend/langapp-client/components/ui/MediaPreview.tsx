import React, { useEffect, useState } from 'react';
import { ActivityIndicator, Alert, Image, Modal, Pressable, View } from 'react-native';
import { Text } from './text';
import { FileIcon, FileTextIcon, ImageIcon, XCircle } from 'lucide-react-native';
import { ExternalLink } from '@/lib/icons/ExternalLink';
import * as WebBrowser from 'expo-linking';
import { Button } from './button';
import { useFileAccess } from '@/hooks/useFileAccess';
import { unescapeUnicode } from '@/lib/utils';

interface MediaPreviewProps {
  url: string;
  index: number;
}

export function MediaPreview({ url, index }: MediaPreviewProps) {
  const [modalVisible, setModalVisible] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isFetchingFilename, setIsFetchingFilename] = useState(false);
  const [accessibleUrl, setAccessibleUrl] = useState<string | null>(null);
  const [displayedFileName, setDisplayedFileName] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const { getReadUrl } = useFileAccess();

  const getFileExtension = (fileUrl: string): string => {
    const cleanedUrl = fileUrl.split('?')[0];
    const filename = cleanedUrl.split('/').pop() || '';
    return filename.split('.').pop()?.toLowerCase() || '';
  };

  const getFileName = async (fileUrl: string): Promise<string> => {
    let filename = fileUrl.split('/').pop() || `File ${index + 1}`;
    try {
      const response = await fetch(fileUrl, { method: 'HEAD' });
      const encodedFileName = response.headers.get('x-ms-meta-originalfilename');
      filename = unescapeUnicode(encodedFileName || '');
    } catch (fetchError) {
      // Fall through to default filename generation if HEAD request fails
    }
    const currentExtension = getFileExtension(fileUrl);

    return filename.length > 25 ? `${filename.substring(0, 20)}...${currentExtension}` : filename;
  };

  useEffect(() => {
    const fetchAccessibleUrlAndFileName = async () => {
      if (url) {
        setIsLoading(true);
        setError(null);
        setAccessibleUrl(null);
        setDisplayedFileName(null);
        try {
          const newUrl = await getReadUrl(url);
          setAccessibleUrl(newUrl);
          if (newUrl) {
            setIsFetchingFilename(true);
            try {
              const fetchedName = await getFileName(newUrl);
              setDisplayedFileName(fetchedName);
            } catch (nameError) {
              setDisplayedFileName(generateFallbackFileName(newUrl));
            }
            setIsFetchingFilename(false);
          }
        } catch (e) {
          const errorMessage = e instanceof Error ? e.message : 'Failed to get accessible URL';
          setError(errorMessage);
          Alert.alert('Error', `Could not load file: ${errorMessage}`);
        }
        setIsLoading(false);
      }
    };
    fetchAccessibleUrlAndFileName();
  }, [url]);

  const generateFallbackFileName = (fileUrl: string): string => {
    const currentExtension = getFileExtension(fileUrl);
    const filename = fileUrl.split('/').pop() || `File ${index + 1}`;
    return filename.length > 25 ? `${filename.substring(0, 20)}...${currentExtension}` : filename;
  };

  const openFile = async () => {
    if (!accessibleUrl) {
      Alert.alert('Error', 'File URL is not available. Cannot open.');
      return;
    }
    const currentlyOpening = !isLoading;
    if (currentlyOpening) setIsLoading(true);
    try {
      await WebBrowser.openURL(accessibleUrl);
    } catch (error) {
      Alert.alert('Error', 'Could not open this file');
    } finally {
      if (currentlyOpening) setIsLoading(false);
    }
  };

  const currentFileUrl = accessibleUrl || url;
  const extension = getFileExtension(currentFileUrl);
  const isImage = ['jpg', 'jpeg', 'png', 'gif', 'webp'].includes(extension);
  const isPdf = extension === 'pdf';

  const getIcon = () => {
    if (isImage) return <ImageIcon size={24} className="text-muted-foreground" />;
    else if (isPdf) return <FileTextIcon size={24} className="text-muted-foreground" />;
    else return <FileIcon size={24} className="text-muted-foreground" />;
  };

  const getFileNameToDisplay = (): string => {
    return (
      displayedFileName ||
      (currentFileUrl ? generateFallbackFileName(currentFileUrl) : `File ${index + 1}`)
    );
  };

  const renderPreviewContent = () => {
    if (isLoading && !accessibleUrl) {
      return (
        <View className="flex-1 items-center justify-center">
          <ActivityIndicator size="large" color="#4F46E5" />
          <Text className="mt-4 text-base font-medium text-primary">Loading file...</Text>
        </View>
      );
    }
    if (error) {
      return (
        <View className="flex-1 items-center justify-center">
          <Text className="max-w-[80%] text-center text-base font-medium text-destructive">
            Error: {error}
          </Text>
        </View>
      );
    }
    if (!accessibleUrl) {
      return (
        <View className="flex-1 items-center justify-center">
          {!isLoading && (
            <Text className="text-center text-base text-muted-foreground">File not available.</Text>
          )}
        </View>
      );
    }
    const finalExtension = getFileExtension(accessibleUrl);
    const finalIsImage = ['jpg', 'jpeg', 'png', 'gif', 'webp'].includes(finalExtension);
    if (finalIsImage) {
      return (
        <View className="mt-8 flex-1 items-center justify-center">
          <Image
            source={{ uri: accessibleUrl }}
            className="h-full w-full rounded-lg"
            resizeMode="contain"
            onError={() => {
              Alert.alert('Error', 'Failed to load image');
            }}
          />
        </View>
      );
    } else {
      return (
        <View className="flex-1 items-center justify-center px-5">
          <View className="mb-5 h-20 w-20 items-center justify-center rounded-lg bg-muted">
            {getIcon()}
          </View>
          <Text className="mb-2 text-center text-lg font-semibold">
            {isFetchingFilename
              ? 'Loading filename...'
              : displayedFileName || generateFallbackFileName(accessibleUrl)}
          </Text>
          <Text className="mb-8 text-center text-base text-muted-foreground">
            This file type cannot be previewed directly.
          </Text>
          <Button
            onPress={openFile}
            disabled={isLoading || isFetchingFilename}
            className="min-w-[180px] flex-row items-center justify-center rounded-lg bg-primary px-6 py-3">
            <ExternalLink size={16} className="mr-2 text-white" />
            <Text className="text-base font-semibold text-white">
              {isLoading && !accessibleUrl
                ? 'Loading...'
                : isLoading
                  ? 'Opening...'
                  : 'Open in Browser'}
            </Text>
          </Button>
        </View>
      );
    }
  };

  return (
    <>
      <Pressable
        onPress={() => setModalVisible(true)}
        className="mb-2 mr-2 max-w-[90vw] flex-row items-center rounded-lg border border-border bg-card p-3 shadow-sm active:bg-muted/60">
        {(isLoading && !accessibleUrl) || isFetchingFilename ? (
          <View className="mr-3 h-20 w-20 items-center justify-center rounded-md bg-muted">
            <ActivityIndicator size="small" color="#4F46E5" />
          </View>
        ) : isImage && accessibleUrl ? (
          <Image
            source={{ uri: accessibleUrl }}
            className="mr-3 h-20 w-20 rounded-md"
            resizeMode="cover"
          />
        ) : (
          <View className="mr-3 h-12 w-12 items-center justify-center rounded-md bg-muted">
            {getIcon()}
          </View>
        )}
        <Text className="text-base font-medium" numberOfLines={1} ellipsizeMode="tail">
          {isFetchingFilename && !displayedFileName ? 'Loading name...' : getFileNameToDisplay()}
        </Text>
      </Pressable>

      <Modal
        animationType="fade"
        transparent={true}
        visible={modalVisible}
        onRequestClose={() => setModalVisible(false)}>
        <Pressable
          className="flex-1 items-center justify-center bg-black/70 px-5"
          onPress={() => setModalVisible(false)}>
          <View
            className="relative h-[75%] w-full rounded-xl bg-card p-5"
            // Prevent closing when clicking content
            pointerEvents="box-none">
            <Pressable
              onPress={() => setModalVisible(false)}
              className="absolute right-4 top-4 z-10 rounded-full bg-white/90 p-1"
              hitSlop={{ top: 20, right: 20, bottom: 20, left: 20 }}>
              <XCircle size={30} className="text-muted-foreground" />
            </Pressable>
            {renderPreviewContent()}
          </View>
        </Pressable>
      </Modal>
    </>
  );
}
