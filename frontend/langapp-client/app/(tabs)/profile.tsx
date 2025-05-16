import { Text } from "@/components/ui/text";
import { View } from "lucide-react-native";

export default function Profile() {
  return (
    <View className="flex-1 items-center justify-center">
      <Text className="text-2xl font-bold">Profile</Text>
      <Text className="mt-2 text-lg text-gray-500">This is the profile screen.</Text>
    </View>
  );
}
