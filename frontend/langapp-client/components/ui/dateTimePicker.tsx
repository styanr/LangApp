import { DateTimePickerAndroid } from '@react-native-community/datetimepicker';
import { Button } from './button';
import { Text } from './text';
import { View } from 'lucide-react-native';

interface DateTimePickerProps {
  date: Date;
  onChange: (event: any, selectedDate: Date | undefined) => void;
  mode: 'date' | 'time';
  className?: string;
}

export const DatePicker = ({ date, onChange, mode, className }: DateTimePickerProps) => {
  const showMode = (currentMode: 'date' | 'time') => {
    DateTimePickerAndroid.open({
      value: date,
      onChange,
      mode: currentMode,
      is24Hour: true,
    });
  };

  const showDatepicker = () => {
    showMode('date');
  };

  const showTimepicker = () => {
    showMode('time');
  };

  return (
    <View className={`${className || ''}`}>
      <Button onPress={showDatepicker}>
        <Text>Select Date</Text>
      </Button>
      <Text>selected: {date.toLocaleString()}</Text>
    </View>
  );
};
