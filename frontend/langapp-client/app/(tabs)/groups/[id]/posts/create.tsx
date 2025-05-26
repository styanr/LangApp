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
import { handleApiError } from '@/lib/errors';

const CreatePostPage = () => {
  const { id: groupId } = useGlobalSearchParams();
  const router = useRouter();
  const { createPost, mutationStatus } = usePosts();
  const fileUpload = useFileUpload();

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

  const onSubmit = async () => {
    if (!title.trim() || !content.trim()) {
      Alert.alert('Validation Error', 'Title and content cannot be empty.');
      return;
    }

    try {
      setIsUploading(true);

      // Upload all media files if any
      let mediaUrls: string[] = [];

      if (mediaFiles.length > 0) {
        // Separate existing URLs from new files that need to be uploaded
        const existingUrls = mediaFiles.filter((file): file is string => typeof file === 'string');
        const newFiles = mediaFiles.filter(
          (file): file is { uri: string; name: string; type: string; preview?: string } =>
            typeof file !== 'string'
        );

        // Add existing URLs directly
        mediaUrls = [...existingUrls];

        // Upload new files
        if (newFiles.length > 0) {
          const uploadPromises = newFiles.map((file) => {
            return fileUpload.upload(file.uri, file.name, file.type);
          });

          const newUrls = await Promise.all(uploadPromises);
          mediaUrls = [...mediaUrls, ...newUrls];
        }
      }

      // Create post with media URLs if available
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
      handleApiError(err);
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <ScrollView className="flex-1 bg-background">
      <View className="flex-1 bg-background px-4 pt-10">
        <Text className="mb-4 text-2xl font-bold">Create Post</Text>
        <Text className="mb-2">Title</Text>
        <Input
          value={title}
          onChangeText={setTitle}
          placeholder="Enter title"
          className="mb-4 rounded border border-input p-2"
        />
        <Text className="mb-2">Content</Text>
        <Textarea
          value={content}
          onChangeText={setContent}
          placeholder="Enter content"
          multiline
          className="mb-4 h-32 rounded border border-input p-2 text-base"
        />

        <Text className="mb-2">Post Type</Text>
        <View className="mb-4 flex-row">
          <Pressable
            onPress={() => setPostType(PostType.Discussion)}
            className={`mr-2 rounded-lg px-4 py-2 ${
              postType === PostType.Discussion ? 'bg-primary' : 'bg-muted'
            }`}>
            <Text
              className={
                postType === PostType.Discussion ? 'text-primary-foreground' : 'text-foreground'
              }>
              Discussion
            </Text>
          </Pressable>
          <Pressable
            onPress={() => setPostType(PostType.Resource)}
            className={`rounded-lg px-4 py-2 ${
              postType === PostType.Resource ? 'bg-primary' : 'bg-muted'
            }`}>
            <Text
              className={
                postType === PostType.Resource ? 'text-primary-foreground' : 'text-foreground'
              }>
              Resource
            </Text>
          </Pressable>
        </View>

        <Text className="mb-2 text-sm text-muted-foreground">
          Media upload is enabled. You can upload images or documents (PDFs).
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
              ? 'Uploading...'
              : mutationStatus.createPost.isLoading
                ? 'Creating...'
                : 'Create'}
          </Text>
        </Button>
      </View>
    </ScrollView>
  );
};

export default CreatePostPage;
