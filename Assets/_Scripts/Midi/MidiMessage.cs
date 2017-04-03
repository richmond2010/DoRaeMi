/// <summary>MIDI message structure.</summary>
public struct MidiMessage {
	/// <summary>MIDI source ID.</summary>
	public uint source;
	/// <summary>MIDI status byte.</summary>
	public byte status;
	/// <summary>MIDI data byte 1.</summary>
	public byte data1;
	/// <summary>MIDI data byte 2.</summary>
	public byte data2;

	public MidiMessage (ulong data) {
		source = (uint)(data & 0xffffffffUL);
		status = (byte)((data >> 32) & 0xff);
		data1 = (byte)((data >> 40) & 0xff);
		data2 = (byte)((data >> 48) & 0xff);
	}

	public override string ToString () {
		const string fmt = "s({0:X2}) d({1:X2},{2:X2}) from {3:X8}";
		return string.Format (fmt, status, data1, data2, source);
	}
}