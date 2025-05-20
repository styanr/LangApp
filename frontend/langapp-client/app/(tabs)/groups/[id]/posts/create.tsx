import { useState } from 'react';
import { View, TextInput, Alert, Pressable, ScrollView, Image } from 'react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import { useGlobalSearchParams, useRouter } from 'expo-router';
import { usePosts } from '@/hooks/usePosts';
import { PostType } from '@/api/orval/langAppApi.schemas';
import { useFileUpload } from '@/hooks/useFileUpload';
import * as ImagePicker from 'expo-image-picker';
import * as DocumentPicker from 'expo-document-picker';
import { FileText, Image as ImageIcon, X } from 'lucide-react-native';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';

const CreatePostPage = () => {
  const { id: groupId } = useGlobalSearchParams();
  const router = useRouter();
  const { createPost, mutationStatus } = usePosts();
  const fileUpload = useFileUpload();

  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [postType, setPostType] = useState<PostType>(PostType.Discussion);
  const [mediaFiles, setMediaFiles] = useState<
    Array<{ uri: string; name: string; type: string; preview?: string }>
  >([]);
  const [isUploading, setIsUploading] = useState(false);

  // const pickImage = async () => {
  //   try {
  //     const result = await ImagePicker.launchImageLibraryAsync({
  //       mediaTypes: ImagePicker.MediaTypeOptions.Images,
  //       allowsEditing: true,
  //       quality: 0.8,
  //     });

  //     if (!result.canceled && result.assets && result.assets.length > 0) {
  //       const asset = result.assets[0];
  //       // Get filename from uri
  //       const uriParts = asset.uri.split('/');
  //       const fileName = uriParts[uriParts.length - 1];

  //       setMediaFiles([
  //         ...mediaFiles,
  //         {
  //           uri: asset.uri,
  //           name: fileName,
  //           type: 'image/jpeg', // Most common format from picker
  //           preview: asset.uri,
  //         },
  //       ]);
  //     }
  //   } catch (error) {
  //     console.error('Error picking image:', error);
  //     Alert.alert('Error', 'Failed to pick image');
  //   }
  // };

  // const pickDocument = async () => {
  //   if (postType !== PostType.Resource) {
  //     Alert.alert('Info', 'Document uploads are only available for Resource posts');
  //     return;
  //   }

  //   try {
  //     const result = await DocumentPicker.getDocumentAsync({
  //       type: 'application/pdf',
  //       copyToCacheDirectory: true,
  //     });

  //     if (result.canceled === false && result.assets && result.assets.length > 0) {
  //       const asset = result.assets[0];
  //       setMediaFiles([
  //         ...mediaFiles,
  //         {
  //           uri: asset.uri,
  //           name: asset.name,
  //           type: 'application/pdf',
  //         },
  //       ]);
  //     }
  //   } catch (error) {
  //     console.error('Error picking document:', error);
  //     Alert.alert('Error', 'Failed to pick document');
  //   }
  // };

  // const removeFile = (index: number) => {
  //   const newFiles = [...mediaFiles];
  //   newFiles.splice(index, 1);
  //   setMediaFiles(newFiles);
  // };

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
        const uploadPromises = mediaFiles.map((file) => {
          // Use 'images' container for images, 'documents' for PDFs
          const containerName = file.type.startsWith('image/') ? 'images' : 'documents';
          return fileUpload.upload(file.uri, file.name, containerName);
        });

        mediaUrls = await Promise.all(uploadPromises);
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
      Alert.alert('Error', 'Failed to create post or upload media.');
    } finally {
      setIsUploading(false);
    }
  };

  return (
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

      <Text className="mb-4 text-sm text-muted-foreground">
        Media upload is enabled. You can upload images or documents (PDFs).
      </Text>

      <Button onPress={onSubmit} disabled={mutationStatus.createPost.isLoading}>
        <Text className="text-sm font-semibold">
          {mutationStatus.createPost.isLoading ? 'Creating...' : 'Create'}
        </Text>
      </Button>
    </View>
  );
};

export default CreatePostPage;
