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
	/// This class contains the texture scaling information for certain formats.
	/// Some BSP formats lack this lump (or it is contained in a different one)
	/// so their cases will be left out.
	/// </summary>
	public class TexInfo {

		public const int S = 0;
		public const int T = 1;
		public static readonly Vector3[] baseAxes = new Vector3[] { new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(0, -1, 0),
		                                                            new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(0, -1, 0),
		                                                            new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, -1),
		                                                            new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, -1),
		                                                            new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(0, 0, -1),
		                                                            new Vector3(0, -1, 0), new Vector3(1, 0, 0), new Vector3(0, 0, -1) };

		public Vector3[] axes { get; private set; }
		public float[] shifts { get; private set; }
		public int flags { get; private set; }
		public int texture { get; private set; }

		/// <summary>
		/// Creates a new <c>TexInfo</c> object from a <c>byte</c> array.
		/// </summary>
		/// <param name="data"><c>byte</c> array to parse</param>
		/// <param name="type">The map type</param>
		/// <exception cref="ArgumentNullException"><paramref name="data" /> was null</exception>
		/// <exception cref="ArgumentException">This structure is not implemented for the given maptype</exception>
		public TexInfo(byte[] data, MapType type) {
			if (data == null) {
				throw new ArgumentNullException();
			}
			axes = new Vector3[2];
			shifts = new float[2];
			axes[S] = new Vector3(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 4), BitConverter.ToSingle(data, 8));
			shifts[S] = BitConverter.ToSingle(data, 12);
			axes[T] = new Vector3(BitConverter.ToSingle(data, 16), BitConverter.ToSingle(data, 20), BitConverter.ToSingle(data, 24));
			shifts[T] = BitConverter.ToSingle(data, 28);
			switch (type) {
				// Excluded engines: Quake 2-based, Quake 3-based
				case MapType.Source17:
				case MapType.Source18:
				case MapType.Source19:
				case MapType.Source20:
				case MapType.Source21:
				case MapType.Source22:
				case MapType.Source23:
				case MapType.Source27:
				case MapType.TacticalIntervention:
				case MapType.Vindictus: {
					texture = BitConverter.ToInt32(data, 68);
					flags = BitConverter.ToInt32(data, 64);
					break;
				}
				case MapType.DMoMaM: {
					texture = BitConverter.ToInt32(data, 92);
					flags = BitConverter.ToInt32(data, 88);
					break;
				}
				case MapType.Quake: {
					texture = BitConverter.ToInt32(data, 32);
					flags = BitConverter.ToInt32(data, 36);
					break;
				}
				case MapType.Nightfire: {
					break;
				}
				default: {
					throw new ArgumentException("Map type " + type + " isn't supported by the TexInfo class.");
				}
			}
		}

		/// <summary>
		/// Creates a new <c>TexInfo</c> object using the passed data.
		/// </summary>
		/// <param name="s">The S texture axis</param>
		/// <param name="SShift">The texture shift on the S axis</param>
		/// <param name="t">The T texture axis</param>
		/// <param name="TShift">The texture shift on the T axis</param>
		/// <param name="flags">The flags for this <c>TexInfo</c></param>
		/// <param name="texture">Index into the texture list for the texture this <c>TexInfo</c> uses</param>
		public TexInfo(Vector3 s, float SShift, Vector3 t, float TShift, int flags, int texture) {
			axes = new Vector3[2];
			axes[S] = s;
			axes[T] = t;
			shifts = new float[2];
			shifts[S] = SShift;
			shifts[T] = TShift;
			this.flags = flags;
			this.texture = texture;
		}

		/// <summary>
		/// Adapted from code in the Quake III Arena source code. Stolen without
		/// permission because it falls under the terms of the GPL v2 license, because I'm not making
		/// any money, just awesome tools.
		/// </summary>
		/// <param name="p"><c>Plane</c> of the surface</param>
		/// <returns>The best matching texture axes for the given <c>Plane</c></returns>
		public static Vector3[] TextureAxisFromPlane(Plane p) {
			int bestaxis = 0;
			double dot; // Current dot product
			double best = 0; // "Best" dot product so far
			for (int i = 0; i < 6; ++i) {
				// For all possible axes, positive and negative
				dot = Vector3.Dot(p.normal, new Vector3(baseAxes[i * 3][0], baseAxes[i * 3][1], baseAxes[i * 3][2]));
				if (dot > best) {
					best = dot;
					bestaxis = i;
				}
			}
			Vector3[] out_Renamed = new Vector3[2];
			out_Renamed[0] = new Vector3(baseAxes[bestaxis * 3 + 1][0], baseAxes[bestaxis * 3 + 1][1], baseAxes[bestaxis * 3 + 1][2]);
			out_Renamed[1] = new Vector3(baseAxes[bestaxis * 3 + 2][0], baseAxes[bestaxis * 3 + 2][1], baseAxes[bestaxis * 3 + 2][2]);
			return out_Renamed;
		}

		/// <summary>
		/// Factory method to parse a <c>byte</c> array into a <c>List</c> of <c>TexInfo</c> objects.
		/// </summary>
		/// <param name="data">The data to parse</param>
		/// <param name="type">The map type</param>
		/// <returns>A <c>List</c> of <c>TexInfo</c> objects</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data" /> was null</exception>
		/// <exception cref="ArgumentException">This structure is not implemented for the given maptype</exception>
		public static List<TexInfo> LumpFactory(byte[] data, MapType type) {
			if (data == null) {
				throw new ArgumentNullException();
			}
			int structLength = 0;
			switch (type) {
				case MapType.Nightfire: {
					structLength = 32;
					break;
				}
				case MapType.Quake: {
					structLength = 40;
					break;
				}
				case MapType.Source17:
				case MapType.Source18:
				case MapType.Source19:
				case MapType.Source20:
				case MapType.Source21:
				case MapType.Source22:
				case MapType.Source23:
				case MapType.Source27:
				case MapType.TacticalIntervention:
				case MapType.Vindictus: {
					structLength = 72;
					break;
				}
				case MapType.DMoMaM: {
					structLength = 96;
					break;
				}
				default: {
					throw new ArgumentException("Map type " + type + " isn't supported by the Leaf lump factory.");
				}
			}
			List<TexInfo> lump = new List<TexInfo>(data.Length / structLength);
			byte[] bytes = new byte[structLength];
			for (int i = 0; i < data.Length / structLength; ++i) {
				Array.Copy(data, (i * structLength), bytes, 0, structLength);
				lump.Add(new TexInfo(bytes, type));
			}
			return lump;
		}
	}
}