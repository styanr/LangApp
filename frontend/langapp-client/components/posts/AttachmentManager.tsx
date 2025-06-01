import React, { useState } from 'react';
import { View, Text, Pressable, Image, Alert, ScrollView } from 'react-native';
import { Button } from '@/components/ui/button';
import { X, PlusCircle } from 'lucide-react-native';
import { FileText } from '@/lib/icons/FileText';
import { ImageIcon } from '@/lib/icons/ImageIcon';
import * as ImagePicker from 'expo-image-picker';
import * as DocumentPicker from 'expo-document-picker';
import { MediaPreview } from '@/components/ui/MediaPreview';
import { useTranslation } from 'react-i18next';

interface MediaFile {
  uri: string;
  name: string;
  type: string;
  preview?: string; // Local preview URL for images
}

interface AttachmentManagerProps {
  existingMedia?: string[];
  onMediaChange: (media: (string | MediaFile)[]) => void;
  allowDocuments?: boolean;
}

export function AttachmentManager({
  existingMedia = [],
  onMediaChange,
  allowDocuments = true,
}: AttachmentManagerProps) {
  const { t } = useTranslation();
  const [newMediaFiles, setNewMediaFiles] = useState<MediaFile[]>([]);
  const [keptExistingMedia, setKeptExistingMedia] = useState<string[]>(existingMedia);

  const updateMedia = (newFiles: MediaFile[], keptMedia: string[]) => {
    setNewMediaFiles(newFiles);
    setKeptExistingMedia(keptMedia);
    onMediaChange([...newFiles, ...keptMedia]);
  };

  const pickImage = async () => {
    try {
      const result = await ImagePicker.launchImageLibraryAsync({
        mediaTypes: ['images'],
        allowsEditing: true,
        quality: 0.8,
      });

      if (!result.canceled && result.assets && result.assets.length > 0) {
        const asset = result.assets[0];
        console.log(asset.uri);

        // Get filename from uri
        const uriParts = asset.uri.split('/');
        const fileName = uriParts[uriParts.length - 1];

        const newFile = {
          uri: asset.uri,
          name: fileName,
          type: asset.mimeType || 'image/jpeg',
          preview: asset.uri,
        };

        updateMedia([...newMediaFiles, newFile], keptExistingMedia);
      }
    } catch (error) {
      console.error('Error picking image:', error);
      Alert.alert(
        String(t('attachmentManager.errorTitle')),
        String(t('attachmentManager.errorPickImage'))
      );
    }
  };

  const pickDocument = async () => {
    if (!allowDocuments) {
      Alert.alert(
        String(t('attachmentManager.infoTitle')),
        String(t('attachmentManager.infoNoDocuments'))
      );
      return;
    }

    try {
      const result = await DocumentPicker.getDocumentAsync({
        type: 'application/pdf',
        copyToCacheDirectory: true,
      });

      if (!result.canceled && result.assets && result.assets.length > 0) {
        const asset = result.assets[0];
        const newFile = {
          uri: asset.uri,
          name: asset.name,
          type: 'application/pdf',
        };

        updateMedia([...newMediaFiles, newFile], keptExistingMedia);
      }
    } catch (error) {
      console.error('Error picking document:', error);
      Alert.alert(
        String(t('attachmentManager.errorTitle')),
        String(t('attachmentManager.errorPickDocument'))
      );
    }
  };

  const removeNewFile = (index: number) => {
    const updatedFiles = [...newMediaFiles];
    updatedFiles.splice(index, 1);
    updateMedia(updatedFiles, keptExistingMedia);
  };

  const removeExistingFile = (index: number) => {
    const updatedExisting = [...keptExistingMedia];
    updatedExisting.splice(index, 1);
    updateMedia(newMediaFiles, updatedExisting);
  };

  return (
    <View className="mb-4">
      <Text className="mb-2 font-medium">{t('attachmentManager.attachmentsLabel')}</Text>

      {/* Display existing media that hasn't been removed */}
      {keptExistingMedia.length > 0 && (
        <View className="mb-3">
          <Text className="mb-1 text-sm text-muted-foreground">
            {t('attachmentManager.currentAttachments')}
          </Text>
          <ScrollView horizontal showsHorizontalScrollIndicator={false}>
            <View className="flex-row">
              {keptExistingMedia.map((url, index) => (
                <View key={`existing-${index}`} className="relative mr-2">
                  <MediaPreview url={url} index={index} />
                  <Pressable
                    onPress={() => removeExistingFile(index)}
                    className="absolute right-1 top-1 rounded-full bg-background/80 p-1">
                    <X size={14} color="#ef4444" />
                  </Pressable>
                </View>
              ))}
            </View>
          </ScrollView>
        </View>
      )}

      {/* Display newly added files that haven't been uploaded yet */}
      {newMediaFiles.length > 0 && (
        <View className="mb-3">
          <Text className="mb-1 text-sm text-muted-foreground">
            {t('attachmentManager.newAttachments')}
          </Text>
          <ScrollView horizontal showsHorizontalScrollIndicator={false}>
            <View className="flex-row">
              {newMediaFiles.map((file, index) => (
                <View
                  key={`new-${index}`}
                  className="relative mr-2 rounded border border-border bg-muted p-2">
                  {file.type.startsWith('image/') && file.preview ? (
                    <Image
                      source={{ uri: file.preview }}
                      style={{ width: 80, height: 80 }}
                      className="rounded"
                    />
                  ) : (
                    <View className="h-20 w-20 items-center justify-center rounded bg-muted">
                      <FileText size={24} color="#6B7280" />
                    </View>
                  )}
                  <Text className="mt-1 max-w-20 text-xs" numberOfLines={1}>
                    {file.name}
                  </Text>
                  <Pressable
                    onPress={() => removeNewFile(index)}
                    className="absolute right-1 top-1 rounded-full bg-background/80 p-1">
                    <X size={14} color="#ef4444" />
                  </Pressable>
                </View>
              ))}
            </View>
          </ScrollView>
        </View>
      )}

      <View className="mt-2 flex-row">
        <Button variant="outline" onPress={pickImage} className="mr-2 flex-row items-center">
          <ImageIcon size={16} className="mr-1" />
          <Text>{t('attachmentManager.addImage')}</Text>
        </Button>
        {allowDocuments && (
          <Button variant="outline" onPress={pickDocument} className="flex-row items-center">
            <FileText size={16} className="mr-1" />
            <Text>{t('attachmentManager.addDocument')}</Text>
          </Button>
        )}
      </View>
    </View>
  );
}
