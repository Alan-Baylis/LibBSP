#if UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5
#define UNITY
#endif

using System;
using System.Collections.Generic;
#if UNITY
using UnityEngine;
#endif

namespace LibBSP {
#if !UNITY
	using Vector3 = Vector3d;
#endif
	/// <summary>
	/// Handles the data needed for one static prop.
	/// </summary>
	public struct SourceStaticProp {

		public Vector3 origin { get; private set; }
		public Vector3 angles { get; private set; }
		public short dictionaryEntry { get; private set; }
		public byte solidity { get; private set; }
		public byte flags { get; private set; }
		public int skin { get; private set; }
		public float minFadeDist { get; private set; }
		public float maxFadeDist { get; private set; }
		public float forcedFadeScale { get; private set; }
		public string targetname { get; private set; }

		/// <summary>
		/// Creates a new <c>SourceStaticProp</c> object from a <c>byte</c> array.
		/// </summary>
		/// <param name="data"><c>byte</c> array to parse</param>
		/// <param name="type">The map type</param>
		/// <exception cref="ArgumentNullException"><paramref name="data" /> was null</exception>
		/// <exception cref="ArgumentException">This structure is not implemented for the given maptype</exception>
		public SourceStaticProp(byte[] data, MapType type, int version) : this() {
			if (data == null) {
				throw new ArgumentNullException();
			}
			origin = Vector3.zero;
			angles = Vector3.zero;
			dictionaryEntry = 0;
			solidity = 0;
			flags = 0;
			skin = 0;
			minFadeDist = 0;
			maxFadeDist = 0;
			forcedFadeScale = 1;
			targetname = null;
			switch (type) {
				case MapType.Source17:
				case MapType.Source18:
				case MapType.Source19:
				case MapType.Source20:
				case MapType.Source21:
				case MapType.Source22:
				case MapType.Source23:
				case MapType.Source27:
				case MapType.TacticalIntervention:
				case MapType.Vindictus:
				case MapType.DMoMaM: {
					switch (version) {
						case 5: {
							if (data.Length == 188) {
								// This is only for The Ship or Bloody Good Time.
								byte[] targetnameBytes = new byte[128];
								Array.Copy(data, 60, targetnameBytes, 0, 128);
								targetname = targetnameBytes.ToNullTerminatedString();
								if (targetname.Length == 0) {
									targetname = null;
								}
							}
							goto case 6;
						}
						case 6:
						case 7:
						case 8:
						case 9:
						case 10: {
							forcedFadeScale = BitConverter.ToSingle(data, 56);
							goto case 4;
						}
						case 4: {
							origin = new Vector3(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 4), BitConverter.ToSingle(data, 8));
							origin = new Vector3(BitConverter.ToSingle(data, 12), BitConverter.ToSingle(data, 16), BitConverter.ToSingle(data, 20));
							dictionaryEntry = BitConverter.ToInt16(data, 24);
							solidity = data[30];
							flags = data[31];
							skin = BitConverter.ToInt32(data, 32);
							minFadeDist = BitConverter.ToSingle(data, 36);
							maxFadeDist = BitConverter.ToSingle(data, 40);
							break;
						}
					}
					break;
				}
				default: {
					throw new ArgumentException("Map type " + type + " isn't supported by the SourceStaticProp class.");
				}
			}
		}

		/// <summary>
		/// Factory method to create a <c>SourceStaticProps</c> object.
		/// </summary>
		/// <param name="data">The data to parse</param>
		/// <param name="type">The map type</param>
		/// <param name="version">The version of the Static Prop lump</param>
		/// <returns>A <c>SourceStaticProps</c> object</returns>
		public static SourceStaticProps LumpFactory(byte[] data, MapType type, int version) {
			return new SourceStaticProps(data, type, version);
		}
	}
}