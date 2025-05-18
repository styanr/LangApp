import React from 'react';
import { User } from 'lucide-react-native';
import { ImageWithFallback } from './ImageWithFallback';

type UserProfilePictureProps = {
  imageUrl?: string | null;
  size?: number;
  imageClassName?: string;
  iconContainerClassName?: string;
};

export const UserProfilePicture = ({
  imageUrl,
  size = 40,
  imageClassName: providedImageClassName = '',
  iconContainerClassName: providedIconContainerClassName = '',
}: UserProfilePictureProps) => {
  const finalImageClassName = `rounded-full ${providedImageClassName}`.trim();
  const finalIconContainerClassName = `rounded-full ${providedIconContainerClassName}`.trim();

  return (
    <ImageWithFallback
      imageUrl={imageUrl}
      fallbackIcon={<User className="text-indigo-600" />}
      width={size}
      height={size}
      imageClassName={finalImageClassName}
      iconContainerClassName={finalIconContainerClassName}
    />
  );
};
