import { useState } from 'react';
import { View, Alert, Pressable, ScrollView } from 'react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import { useGlobalSearchParams, useRouter } from 'expo-router';
import { usePosts } from '@/hooks/usePosts';
import { PostType } from '@/api/orval/langAppApi.schemas';
import { useFileUpload } from '@/hooks/useFileUpload';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { AttachmentManager } from '@/components/posts/AttachmentManager';
import { useErrorHandler } from '@/hooks/useErrorHandler';
import { useTranslation } from 'react-i18next';
import Toast from 'react-native-toast-message';

const CreatePostPage = () => {
  const { id: groupId } = useGlobalSearchParams();
  const router = useRouter();
  const { createPost, mutationStatus } = usePosts();
  const fileUpload = useFileUpload();
  const { t } = useTranslation();
  const { handleError } = useErrorHandler();

  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [postType, setPostType] = useState<PostType>(PostType.Discussion);
  const [mediaFiles, setMediaFiles] = useState<
    Array<{ uri: string; name: string; type: string; preview?: string } | string>
  >([]);
  const [isUploading, setIsUploading] = useState(false);

  // Handle media files changes from AttachmentManager
  const handleMediaChange = (
    updatedMedia: (string | { uri: string; name: string; type: string; preview?: string })[]
  ) => {
    setMediaFiles(updatedMedia);
  };

  const hasDocuments = () => {
    return mediaFiles.some((file) => {
      if (typeof file === 'string') {
        // For existing media URLs, check if they end with document extensions
        const url = file.toLowerCase();
        return url.includes('.pdf') || url.includes('application/pdf');
      } else {
        return file.type === 'application/pdf' || file.type.includes('pdf');
      }
    });
  };

  const handlePostTypeChange = (newPostType: PostType) => {
    // Prevent switching from Resource to Discussion if documents are attached
    if (postType === PostType.Resource && newPostType === PostType.Discussion && hasDocuments()) {
      Toast.show({
        type: 'error',
        text1: t('createPostScreen.cannotSwitchTitle'),
        text2: t('createPostScreen.cannotSwitchMessage'),
        position: 'top',
      });
      return;
    }

    setPostType(newPostType);
  };

  const onSubmit = async () => {
    if (!title.trim() || !content.trim()) {
      Alert.alert(
        t('createPostScreen.validationErrorTitle'),
        t('createPostScreen.validationErrorMessage')
      );
      return;
    }

    try {
      setIsUploading(true);

      let mediaUrls: string[] = [];

      if (mediaFiles.length > 0) {
        // Separate existing URLs from new files that need to be uploaded
        const existingUrls = mediaFiles.filter((file): file is string => typeof file === 'string');
        const newFiles = mediaFiles.filter(
          (file): file is { uri: string; name: string; type: string; preview?: string } =>
            typeof file !== 'string'
        );

        mediaUrls = [...existingUrls];

        if (newFiles.length > 0) {
          const uploadPromises = newFiles.map((file) => {
            return fileUpload.upload(file.uri, file.name, file.type);
          });

          const newUrls = await Promise.all(uploadPromises);
          mediaUrls = [...mediaUrls, ...newUrls];
        }
      }

      await createPost({
        title,
        content,
        groupId: groupId as string,
        type: postType,
        media: mediaUrls.length > 0 ? mediaUrls : null,
      });

      router.back();
    } catch (err) {
      console.error('Error creating post:', err);
      // console.error(JSON.stringify(err.response.data, null, 2));
      handleError(err);
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <ScrollView className="flex-1 bg-background">
      <View className="flex-1 bg-background px-4 pt-10">
        <Text className="mb-4 text-2xl font-bold">{t('createPostScreen.title')}</Text>
        <Text className="mb-2">{t('createPostScreen.titleLabel')}</Text>
        <Input
          value={title}
          onChangeText={setTitle}
          placeholder={t('createPostScreen.titlePlaceholder') as string}
          className="mb-4 rounded border border-input p-2"
        />
        <Text className="mb-2">{t('createPostScreen.contentLabel')}</Text>
        <Textarea
          value={content}
          onChangeText={setContent}
          placeholder={t('createPostScreen.contentPlaceholder') as string}
          multiline
          className="mb-4 h-32 rounded border border-input p-2 text-base"
        />

        <Text className="mb-2">{t('createPostScreen.postTypeLabel')}</Text>
        <View className="mb-4 flex-row">
          <Pressable
            onPress={() => handlePostTypeChange(PostType.Discussion)}
            className={`mr-2 rounded-lg px-4 py-2 ${
              postType === PostType.Discussion ? 'bg-primary' : 'bg-muted'
            }`}>
            <Text
              className={
                postType === PostType.Discussion ? 'text-primary-foreground' : 'text-foreground'
              }>
              {t('createPostScreen.discussionType')}
            </Text>
          </Pressable>
          <Pressable
            onPress={() => handlePostTypeChange(PostType.Resource)}
            className={`rounded-lg px-4 py-2 ${
              postType === PostType.Resource ? 'bg-primary' : 'bg-muted'
            }`}>
            <Text
              className={
                postType === PostType.Resource ? 'text-primary-foreground' : 'text-foreground'
              }>
              {t('createPostScreen.resourceType')}
            </Text>
          </Pressable>
        </View>

        <Text className="mb-2 text-sm text-muted-foreground">
          {t('createPostScreen.mediaHint')}
        </Text>

        {/* Attachment manager for adding and removing files */}
        <AttachmentManager
          onMediaChange={handleMediaChange}
          allowDocuments={postType === PostType.Resource}
        />

        <Button
          className="mt-4"
          onPress={onSubmit}
          disabled={
            mutationStatus.createPost.isLoading || isUploading || !title.trim() || !content.trim()
          }>
          <Text className="text-sm font-semibold">
            {isUploading
              ? t('createPostScreen.uploading')
              : mutationStatus.createPost.isLoading
                ? t('createPostScreen.creating')
                : t('createPostScreen.createButton')}
          </Text>
        </Button>
      </View>
    </ScrollView>
  );
};

export default CreatePostPage;
