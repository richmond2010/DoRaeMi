/// <summary>MIDI Manager</summary>
public static class MidiManager {

        // Returns the key state (on: velocity, off: zero).
        public static float GetKey(MidiChannel channel, int noteNumber)
        {
            return MidiDriver.Instance.GetKey(channel, noteNumber);
        }

        public static float GetKey(int noteNumber)
        {
            return MidiDriver.Instance.GetKey(MidiChannel.All, noteNumber);
        }

        public static bool GetKeyDown(int noteNumber)
        {
            return MidiDriver.Instance.GetKeyDown(MidiChannel.All, noteNumber);
        }
}
