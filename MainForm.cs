using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace HaloAnimationEditor
{
	public class MainForm : System.Windows.Forms.Form
	{
		#region Block Counters
		int ObjectsCount = 0;
		int UnitsCount = 0;

		int WeaponsCount = 0;
		int WeaponsAnimationsCount = 0;
		int VehiclesCount = 0;
		int VehiclesAnimationsCount = 0;
		int VehiclesSuspensionAnimationsCount = 0;
		int DevicesCount = 0;
		int DevicesAnimationsCount = 0;
		int UnitDamageCount = 0;
		int FPWeaponCount = 0;
		int FPWeaponAnimationsCount = 0;
		int SoundRefCount = 0;
		int NodesCount = 0;
		private System.Windows.Forms.FolderBrowserDialog BatchExtract;
		int AnimationsCount = 0;

		void ResetCounters()
		{
			this.ObjectsCount = 0;
			this.UnitsCount = 0;

			this.WeaponsCount = 0;
			this.WeaponsAnimationsCount = 0;
			this.VehiclesCount = 0;
			this.VehiclesAnimationsCount = 0;
			this.VehiclesSuspensionAnimationsCount = 0;
			this.DevicesCount = 0;
			this.DevicesAnimationsCount = 0;
			this.UnitDamageCount = 0;
			this.FPWeaponCount = 0;
			this.FPWeaponAnimationsCount = 0;
			this.SoundRefCount = 0;
			this.NodesCount = 0;
			this.AnimationsCount = 0;
		}

		#endregion Block Counters
		
		#region Block stuff
		struct UnitsWeaponTypes
		{
			public int UnitsWeaponTypesAnimationsCount;
		}

		struct UnitsWeapons
		{
			public int UnitsWeaponsAnimationCount;
			public int UnitsWeaponsIKPointsCount;
			public UnitsWeaponTypes[] UnitsWeaponTypesCount;
		}
		
		struct Units
		{
			public int UnitsAnimationsCount;
			public int UnitsIKPointsCount;
			public UnitsWeapons[] UnitsWeaponsCount;
		}

		struct SoundRefrences
		{
			public int Size;
		}

		public class AnimationBlock
		{
			public byte[] rawData;
			public byte[] frameInfo;
			public byte[] defaultData;
			public byte[] frameData;
			public byte[] block;  // 180 bytes
			public string Name 
			{
				get 
				{
					string s = "";
					for (int x = 0; x < block.Length - 1; x++)
					{
						if(block[x] == 0) break;
						byte[] b = new byte[1];
						b[0] = block[x];
						s = s + System.Text.Encoding.Default.GetString(b);
					}
					return s;
				}
			}
			public uint nodeChecksum 
			{
				get 
				{
					byte[] b = new byte[4];
					b[0] = block[40];	b[1] = block[41];	b[2] = block[42];	b[3] = block[43];
					return (uint)ValueSwap.BytesToInt(b);
				}
				set 
				{
					byte[] b = ValueSwap.IntToBytes((int)value);
					block[40] = b[0]; block[41] = b[1]; block[42] = b[2];	block[43] = b[3];
				}
			}

			public int FrameInfo 
			{
				get 
				{
					byte[] b = new byte[4];
					b[0] = block[72];	b[1] = block[73];	b[2] = block[74];	b[3] = block[75];
					return ValueSwap.BytesToInt(b);
				}
				set 
				{
					byte[] b = ValueSwap.IntToBytes(value);
					block[72] = b[0]; block[73] = b[1]; block[74] = b[2];	block[75] = b[3];
				}
			}
			public int DefaultData 
			{
				get 
				{
					byte[] b = new byte[4];
					b[0] = block[140];	b[1] = block[141];	b[2] = block[142];	b[3] = block[143];
					return ValueSwap.BytesToInt(b);
				}
				set 
				{
					byte[] b = ValueSwap.IntToBytes(value);
					block[140] = b[0]; block[141] = b[1]; block[142] = b[2];	block[143] = b[3];
				}
			}
			public int FrameData 
			{
				get 
				{
					byte[] b = new byte[4];
					b[0] = block[160];	b[1] = block[161];	b[2] = block[162];	b[3] = block[163];
					return ValueSwap.BytesToInt(b);
				}
				set 
				{
					byte[] b = ValueSwap.IntToBytes(value);
					block[160] = b[0]; block[161] = b[1]; block[162] = b[2];	block[163] = b[3];
				}
			}

			public int TotalData
			{
				get { return FrameInfo + DefaultData + FrameData; }
			}

			public AnimationBlock()
			{
				block = new byte[180];
			}
			public void ReadMeta(ref BinaryReader br)
			{
				block = br.ReadBytes(180);
			}
			public void ReadData(ref BinaryReader br)
			{
				// Swap all this shiznit
				//int iFrameInfo = SwapInt(FrameInfo);
				//int iDefaultData = SwapInt(DefaultData);
				//int iFrameData = SwapInt(FrameData);

				//int rawDataSize = iFrameInfo + iDefaultData + iFrameData;
				int rawDataSize = FrameInfo + DefaultData + FrameData;
				//int rawDataSize = DefaultData + FrameData;
				rawData = new byte[rawDataSize];
				frameInfo = br.ReadBytes(FrameInfo);
				defaultData = br.ReadBytes(DefaultData);
				frameData = br.ReadBytes(FrameData);
				br.BaseStream.Position -= rawDataSize;
				rawData = br.ReadBytes(rawDataSize);
			}
			public void WriteMeta(ref BinaryWriter bw)
			{				
				bw.Write(block);
			}
			public void WriteData(ref BinaryWriter bw)
			{
				bw.Write(rawData);
			}

			public void WriteFrameInfo(BinaryWriter bw)
			{
//				for(int x = 0; x < FrameInfo; x++)
//				{
//					bw.Write(rawData[x]);
//				}
				bw.Write(frameInfo);
				bw.Flush();
				bw.Close();
			}

			public void WriteDefaultData(BinaryWriter bw)
			{
//				for(int x = FrameInfo; x < DefaultData; x++)
//				{
//					bw.Write(rawData[x]);
//				}
				bw.Write(defaultData);
				bw.Flush();
				bw.Close();
			}

			public void WriteFrameData(BinaryWriter bw)
			{
//				for(int x = DefaultData; x < FrameData; x++)
//				{
//					bw.Write(rawData[x]);
//				}
				bw.Write(frameData);
				bw.Flush();
				bw.Close();
			}
			public void WriteFrameInfo(ref BinaryWriter bw)
			{
//				for(int x = 0; x < FrameInfo; x++)
//				{
//					bw.Write(rawData[x]);
//				}
				bw.Write(frameInfo);
				bw.Flush();
			}

			public void WriteDefaultData(ref BinaryWriter bw)
			{
//				for(int x = FrameInfo; x < DefaultData; x++)
//				{
//					bw.Write(rawData[x]);
//				}
				bw.Write(defaultData);
				bw.Flush();
			}

			public void WriteFrameData(ref BinaryWriter bw)
			{
//				for(int x = DefaultData; x < FrameData; x++)
//				{
//					bw.Write(rawData[x]);
//				}
				bw.Write(frameData);
				bw.Flush();
			}
		};
		#endregion

		#region Animation Util
		// Skip through all of the other blocks to the Animations block
		byte[] SkipAnimationBody(ref System.IO.BinaryReader br)
		{
			br.ReadBytes(64); // Read past the Guerilla Tag Header

			#region Read Blocks
			ObjectsCount = ValueSwap.SwapLong(br.ReadInt32());
			br.ReadBytes(8);

			UnitsCount = ValueSwap.SwapLong(br.ReadInt32());
			br.ReadBytes(8);

			WeaponsCount = ValueSwap.SwapLong(br.ReadInt32());
			br.ReadBytes(8);

			VehiclesCount = ValueSwap.SwapLong(br.ReadInt32());
			br.ReadBytes(8);

			DevicesCount = ValueSwap.SwapLong(br.ReadInt32());
			br.ReadBytes(8);

			UnitDamageCount = ValueSwap.SwapLong(br.ReadInt32());
			br.ReadBytes(8);

			FPWeaponCount = ValueSwap.SwapLong(br.ReadInt32());
			br.ReadBytes(8);

			SoundRefCount = ValueSwap.SwapLong(br.ReadInt32());
			SoundRefrences[] SoundRefs = new SoundRefrences[SoundRefCount];
			br.ReadBytes(16);

			NodesCount = ValueSwap.SwapLong(br.ReadInt32());
			br.ReadBytes(8);

			AnimationsCount = ValueSwap.SwapLong(br.ReadInt32());
			br.ReadBytes(8);

			if(AnimationsCount == 0)
			{
				MessageBox.Show(this, "There are no animations in the tag", "Whoops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				OnUnload(null, null);
			}
			#endregion Read Blocks

			#region Objects

			// Skip Objects Block
			for(int x = 0; x < this.ObjectsCount; x++)
				br.ReadBytes(20);
			#endregion Objects

			#region Units

			Units[] UnitsBlock = new Units[UnitsCount];

			#region UnitsBlock
			for(int x = 0; x < this.UnitsCount; x++)
			{
				br.ReadBytes(64);

				UnitsBlock[x].UnitsAnimationsCount = ValueSwap.SwapLong(br.ReadInt32());
				br.ReadBytes(8);				

				UnitsBlock[x].UnitsIKPointsCount = ValueSwap.SwapLong(br.ReadInt32());
				br.ReadBytes(8);
				
				UnitsBlock[x].UnitsWeaponsCount = new UnitsWeapons[ValueSwap.SwapLong(br.ReadInt32())];
				br.ReadBytes(8);
			}
			#endregion

			for(int x = 0; x < this.UnitsCount; x++)
			{
				// Skip UnitsAnimations Block
				#region UnitsAnimations
				for(int x1 = 0; x1 < UnitsBlock[x].UnitsAnimationsCount; x1++)
					br.ReadBytes(2);
				#endregion

				// Skip UnitsIKPoints block
				#region UnitsIKPoints
				for(int x2 = 0; x2 < UnitsBlock[x].UnitsIKPointsCount; x2++)
					br.ReadBytes(64);
				#endregion

				// Skip UnitsWeapons block
				#region UnitsWeapons
				for(int x3 = 0; x3 < UnitsBlock[x].UnitsWeaponsCount.Length; x3++)
				{
					br.ReadBytes(152);

					UnitsBlock[x].UnitsWeaponsCount[x3].UnitsWeaponsAnimationCount = ValueSwap.SwapLong(br.ReadInt32());
					br.ReadBytes(8);

					UnitsBlock[x].UnitsWeaponsCount[x3].UnitsWeaponsIKPointsCount = ValueSwap.SwapLong(br.ReadInt32());
					br.ReadBytes(8);

					UnitsBlock[x].UnitsWeaponsCount[x3].UnitsWeaponTypesCount = new UnitsWeaponTypes[ValueSwap.SwapLong(br.ReadInt32())];
					br.ReadBytes(8);
				}

				for(int x3 = 0; x3 < UnitsBlock[x].UnitsWeaponsCount.Length; x3++)
				{
					// Skip UnitsWeaponsAnimation block
					#region UnitsWeaponsAnimation
					for(int x4 = 0; x4 < UnitsBlock[x].UnitsWeaponsCount[x3].UnitsWeaponsAnimationCount; x4++)
						br.ReadBytes(2);
					#endregion

					// Skip UnitsWeaponIKPoints block
					#region UnitsWeaponIKPoints
					for(int x5 = 0; x5 < UnitsBlock[x].UnitsWeaponsCount[x3].UnitsWeaponsIKPointsCount; x5++)
						br.ReadBytes(64);
					#endregion

					// Skip WeaponTypes Block
					#region WeaponTypes
					for(int x6 = 0; x6 < UnitsBlock[x].UnitsWeaponsCount[x3].UnitsWeaponTypesCount.Length; x6++)
					{
						br.ReadBytes(48);
						UnitsBlock[x].UnitsWeaponsCount[x3].UnitsWeaponTypesCount[x6].UnitsWeaponTypesAnimationsCount = ValueSwap.SwapLong(br.ReadInt32());
						br.ReadBytes(8);	
					}
					#endregion

					// UnitsWeaponTypesAnimations block
					#region UnitsWeaponTypesAnimations
					for(int x6 = 0; x6 < UnitsBlock[x].UnitsWeaponsCount[x3].UnitsWeaponTypesCount.Length; x6++)
					{
						for(int x7 = 0; x7 < UnitsBlock[x].UnitsWeaponsCount[x3].UnitsWeaponTypesCount[x6].UnitsWeaponTypesAnimationsCount; x7++)
							br.ReadBytes(2);
					}
					#endregion
				}
				#endregion
					
			}
			#endregion Units

			#region Weapons

			// Skip Weapons Block
			for(int x = 0; x < this.WeaponsCount; x++)
			{
				br.ReadBytes(16);
				this.WeaponsAnimationsCount = ValueSwap.SwapLong(br.ReadInt32());
				br.ReadBytes(8);

				// Skip WeaponsAnimations
				for(int x1 = 0; x1 < this.WeaponsAnimationsCount; x1++)
					br.ReadBytes(2);
			}
			#endregion Weapons

			#region Vehicles
			// Skip Vehicles Block
			for(int x = 0; x < this.VehiclesCount; x++)
			{
				br.ReadBytes(92);

				this.VehiclesAnimationsCount = ValueSwap.SwapLong(br.ReadInt32());
				br.ReadBytes(8);
				
				this.VehiclesSuspensionAnimationsCount = ValueSwap.SwapLong(br.ReadInt32());
				br.ReadBytes(8);

				// Skip VehiclesAnimations
				for(int x1 = 0; x1 < this.VehiclesAnimationsCount; x1++)
					br.ReadBytes(2);

				// Skip VehiclesSuspensionAnimations
				for(int x2 = 0; x2 < this.VehiclesSuspensionAnimationsCount; x2++)
					br.ReadBytes(20);
			}
			#endregion Vehicles

			#region Devices

			// Skip Devices Block
			for(int x = 0; x < this.DevicesCount; x++)
			{
				br.ReadBytes(84);

				this.DevicesAnimationsCount = ValueSwap.SwapLong(br.ReadInt32());
				br.ReadBytes(8);

				// Skip DeviceAnimations
				for(int x1 = 0; x1 < this.DevicesAnimationsCount; x1++)
					br.ReadBytes(2);
			}
			#endregion Devices

			#region UnitDamage

			// Skip Unit Damage Block
			for(int x = 0; x < this.UnitDamageCount; x++)
				br.ReadBytes(2);

			#endregion UnitDamage

			#region FPWeapon

			// Skip FP Weapon Block
			for(int x = 0; x < this.FPWeaponCount; x++)
			{
				br.ReadBytes(16);

				this.FPWeaponAnimationsCount = ValueSwap.SwapLong(br.ReadInt32());
				br.ReadBytes(8);

				// Skip FPWeaponAnimations
				for(int x1 = 0; x1 < this.FPWeaponAnimationsCount; x1++)
					br.ReadBytes(2);
			}

			if(FPWeaponCount != 0)
			{
				Trace.WriteLine(br.BaseStream.Position.ToString());
				Trace.WriteLine("");
			}
			#endregion FPWeapon

			#region SoundRef

			// Skip Sound Ref Block
			for(int x = 0; x < this.SoundRefCount; x++)
			{
				br.ReadBytes(8);
				// Read how long the reference string is
				SoundRefs[x].Size = ValueSwap.SwapLong(br.ReadInt32()) + 1;
				br.ReadBytes(8);
			}

			int totalbytes = 0;
			for(int x = 0; x < this.SoundRefCount; x++)
				totalbytes += SoundRefs[x].Size;

			br.ReadBytes(totalbytes);

			#endregion SoundRef

			#region Nodes

			// Skip Nodes Block
			for(int x = 0; x < this.NodesCount; x++)
				br.ReadBytes(64);

			#endregion Nodes

			long offset = br.BaseStream.Position;
			br.BaseStream.Seek(0, SeekOrigin.Begin);
			byte[] header = br.ReadBytes((int)offset);
			return header;
		}

		AnimationBlock[] ReadAnimations(ref System.IO.BinaryReader br)
		{
			AnimationBlock[] Animations = new AnimationBlock[AnimationsCount];
			for(int x = 0; x < this.AnimationsCount; x++)
				Animations[x] = new AnimationBlock();

			// Read the Animations blocks
			for(int x = 0; x < this.AnimationsCount; x++)
				Animations[x].ReadMeta(ref br);

			// Read Animation Data
			for(int x = 0; x < this.AnimationsCount; x++)
				Animations[x].ReadData(ref br);

			return Animations;
		}

		int FindAnimation(string s, AnimationBlock[] array)
		{
			int index = 0, x = 0;
			foreach(AnimationBlock anime in array)
			{
				if(anime.Name.Equals(s)) index = x;

				x++;
			}
			return index;
		}

		#endregion

		#region Animation objects
		// Our Streams
		BinaryReader target;
		BinaryReader source;

		AnimationBlock[] TargetAnimations;
		byte[] TargetHeader;
		AnimationBlock[] SourceAnimations;
		byte[] SourceHeader;
		BinaryWriter output;
		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.MainMenu MainMenu;
		private System.Windows.Forms.MenuItem FileMenu;
		private System.Windows.Forms.MenuItem FileExit;
		private System.Windows.Forms.MenuItem AboutMenu;
		private System.Windows.Forms.MenuItem AboutAbout;
		private System.Windows.Forms.OpenFileDialog OpenFileDialog;
		private System.Windows.Forms.SaveFileDialog SaveFileDialog;
		private HaloAnimationEditor.MAETagReference TargetTagRef;
		private HaloAnimationEditor.MAETagReference SourceTagRef;
		private System.Windows.Forms.Button LoadButton;
		private System.Windows.Forms.Button UnLoad;
		private System.Windows.Forms.Button InsertAnimation;
		private System.Windows.Forms.Button TExtractFrameInfo;
		private System.Windows.Forms.Button TExtractDefaultData;
		private System.Windows.Forms.Button TExtractFrameData;
		private System.Windows.Forms.Button TExport;
		private System.Windows.Forms.Button TExtractAnimation;
		private System.Windows.Forms.Button SExtractAnimation;
		private System.Windows.Forms.Button SExtractFrameData;
		private System.Windows.Forms.Button SExtractDefaultData;
		private System.Windows.Forms.Button SExtractFrameInfo;
		private System.Windows.Forms.Button TargetBatchFrameInfo;
		private System.Windows.Forms.Button TargetBatchDefaultData;
		private System.Windows.Forms.Button TargetBatchFrameData;
		private System.Windows.Forms.Button SourceBatchFrameData;
		private System.Windows.Forms.Button SourceBatchDefaultData;
		private System.Windows.Forms.Button SourceBatchFrameInfo;
		private System.Windows.Forms.Button SExport;
		private System.Windows.Forms.Button Compile;
		private System.Windows.Forms.ComboBox TargetAnimationBlock;
		private System.Windows.Forms.ComboBox SourceAnimationBlock;
		private System.Windows.Forms.SaveFileDialog ExtractDialog;
		private System.Windows.Forms.ColumnHeader SourceColumn;
		private System.Windows.Forms.ColumnHeader TargetColumn;
		private System.Windows.Forms.ListView ActionList;
		private System.Windows.Forms.CheckBox FixNodes;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
			InitializeComponent();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
					components.Dispose();
			}
			base.Dispose( disposing );

			// Close our Streams
			if(target != null)
				target.Close();
			if(source != null)
				source.Close();
			if(output != null)
				output.Close();
		}

		
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.MainMenu = new System.Windows.Forms.MainMenu();
			this.FileMenu = new System.Windows.Forms.MenuItem();
			this.FileExit = new System.Windows.Forms.MenuItem();
			this.AboutMenu = new System.Windows.Forms.MenuItem();
			this.AboutAbout = new System.Windows.Forms.MenuItem();
			this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.TargetTagRef = new HaloAnimationEditor.MAETagReference();
			this.SourceTagRef = new HaloAnimationEditor.MAETagReference();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.LoadButton = new System.Windows.Forms.Button();
			this.TExtractFrameInfo = new System.Windows.Forms.Button();
			this.TExtractDefaultData = new System.Windows.Forms.Button();
			this.TExtractFrameData = new System.Windows.Forms.Button();
			this.TExport = new System.Windows.Forms.Button();
			this.UnLoad = new System.Windows.Forms.Button();
			this.InsertAnimation = new System.Windows.Forms.Button();
			this.TExtractAnimation = new System.Windows.Forms.Button();
			this.SExtractAnimation = new System.Windows.Forms.Button();
			this.SExtractFrameData = new System.Windows.Forms.Button();
			this.SExtractDefaultData = new System.Windows.Forms.Button();
			this.SExtractFrameInfo = new System.Windows.Forms.Button();
			this.SExport = new System.Windows.Forms.Button();
			this.TargetAnimationBlock = new System.Windows.Forms.ComboBox();
			this.SourceAnimationBlock = new System.Windows.Forms.ComboBox();
			this.ExtractDialog = new System.Windows.Forms.SaveFileDialog();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.ActionList = new System.Windows.Forms.ListView();
			this.SourceColumn = new System.Windows.Forms.ColumnHeader();
			this.TargetColumn = new System.Windows.Forms.ColumnHeader();
			this.Compile = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.FixNodes = new System.Windows.Forms.CheckBox();
			this.TargetBatchFrameInfo = new System.Windows.Forms.Button();
			this.TargetBatchDefaultData = new System.Windows.Forms.Button();
			this.TargetBatchFrameData = new System.Windows.Forms.Button();
			this.SourceBatchFrameData = new System.Windows.Forms.Button();
			this.SourceBatchDefaultData = new System.Windows.Forms.Button();
			this.SourceBatchFrameInfo = new System.Windows.Forms.Button();
			this.BatchExtract = new System.Windows.Forms.FolderBrowserDialog();
			this.SuspendLayout();
			// 
			// MainMenu
			// 
			this.MainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.FileMenu,
																					 this.AboutMenu});
			// 
			// FileMenu
			// 
			this.FileMenu.Index = 0;
			this.FileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.FileExit});
			this.FileMenu.Text = "File";
			// 
			// FileExit
			// 
			this.FileExit.Index = 0;
			this.FileExit.Text = "Exit";
			this.FileExit.Click += new System.EventHandler(this.OnExit);
			// 
			// AboutMenu
			// 
			this.AboutMenu.Index = 1;
			this.AboutMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.AboutAbout});
			this.AboutMenu.Text = "About";
			// 
			// AboutAbout
			// 
			this.AboutAbout.Index = 0;
			this.AboutAbout.Text = "About...";
			this.AboutAbout.Click += new System.EventHandler(this.OnAbout);
			// 
			// TargetTagRef
			// 
			this.TargetTagRef.ControlName = "Target Animation Tag";
			this.TargetTagRef.Field = "";
			this.TargetTagRef.FilterText = "Animation Tag|*.model_animations";
			this.TargetTagRef.Info = "";
			this.TargetTagRef.Location = new System.Drawing.Point(0, 8);
			this.TargetTagRef.Name = "TargetTagRef";
			this.TargetTagRef.NoTagType = false;
			this.TargetTagRef.Size = new System.Drawing.Size(576, 24);
			this.TargetTagRef.TabIndex = 0;
			this.TargetTagRef.TagType = "model_animations";
			this.toolTip.SetToolTip(this.TargetTagRef, "The animation tag you want to edit");
			// 
			// SourceTagRef
			// 
			this.SourceTagRef.ControlName = "Source Animation Tag";
			this.SourceTagRef.Field = "";
			this.SourceTagRef.FilterText = "Animation Tag|*.model_animations";
			this.SourceTagRef.Info = "";
			this.SourceTagRef.Location = new System.Drawing.Point(0, 32);
			this.SourceTagRef.Name = "SourceTagRef";
			this.SourceTagRef.NoTagType = false;
			this.SourceTagRef.Size = new System.Drawing.Size(576, 24);
			this.SourceTagRef.TabIndex = 1;
			this.SourceTagRef.TagType = "model_animations";
			this.toolTip.SetToolTip(this.SourceTagRef, "The animation tag that has the animations you want to add to the target animation" +
				"");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 24);
			this.label1.TabIndex = 4;
			this.label1.Text = "Target Animation Block:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(0, 200);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(136, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "Source Animation Block:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// LoadButton
			// 
			this.LoadButton.Location = new System.Drawing.Point(208, 56);
			this.LoadButton.Name = "LoadButton";
			this.LoadButton.Size = new System.Drawing.Size(104, 23);
			this.LoadButton.TabIndex = 2;
			this.LoadButton.Text = "Load Animations";
			this.toolTip.SetToolTip(this.LoadButton, "Load the animation tag into memory for editing");
			this.LoadButton.Click += new System.EventHandler(this.OnLoad);
			// 
			// TExtractFrameInfo
			// 
			this.TExtractFrameInfo.Enabled = false;
			this.TExtractFrameInfo.Location = new System.Drawing.Point(312, 96);
			this.TExtractFrameInfo.Name = "TExtractFrameInfo";
			this.TExtractFrameInfo.Size = new System.Drawing.Size(128, 23);
			this.TExtractFrameInfo.TabIndex = 5;
			this.TExtractFrameInfo.Text = "Extract Frame Info";
			this.toolTip.SetToolTip(this.TExtractFrameInfo, "Extract the Current Target\'s Animation\'s Frame Info");
			this.TExtractFrameInfo.Click += new System.EventHandler(this.OnTExtractFrameInfo);
			// 
			// TExtractDefaultData
			// 
			this.TExtractDefaultData.Enabled = false;
			this.TExtractDefaultData.Location = new System.Drawing.Point(440, 96);
			this.TExtractDefaultData.Name = "TExtractDefaultData";
			this.TExtractDefaultData.Size = new System.Drawing.Size(128, 23);
			this.TExtractDefaultData.TabIndex = 6;
			this.TExtractDefaultData.Text = "Extract Default Data";
			this.toolTip.SetToolTip(this.TExtractDefaultData, "Extract the Current Target\'s Animation\'s Default Data");
			this.TExtractDefaultData.Click += new System.EventHandler(this.OnTExtractDefaultData);
			// 
			// TExtractFrameData
			// 
			this.TExtractFrameData.Enabled = false;
			this.TExtractFrameData.Location = new System.Drawing.Point(312, 120);
			this.TExtractFrameData.Name = "TExtractFrameData";
			this.TExtractFrameData.Size = new System.Drawing.Size(128, 23);
			this.TExtractFrameData.TabIndex = 7;
			this.TExtractFrameData.Text = "Extract Frame Data";
			this.toolTip.SetToolTip(this.TExtractFrameData, "Extract the Current Target\'s Animation\'s Frame Data");
			this.TExtractFrameData.Click += new System.EventHandler(this.OnTExtractFrameData);
			// 
			// TExport
			// 
			this.TExport.Enabled = false;
			this.TExport.Location = new System.Drawing.Point(312, 144);
			this.TExport.Name = "TExport";
			this.TExport.Size = new System.Drawing.Size(256, 23);
			this.TExport.TabIndex = 9;
			this.TExport.Text = "Export Animation As...";
			this.toolTip.SetToolTip(this.TExport, "Export the Current Target\'s Animation to 3ds max Studio 5\\6");
			this.TExport.Click += new System.EventHandler(this.OnTExport);
			// 
			// UnLoad
			// 
			this.UnLoad.Enabled = false;
			this.UnLoad.Location = new System.Drawing.Point(312, 56);
			this.UnLoad.Name = "UnLoad";
			this.UnLoad.Size = new System.Drawing.Size(104, 23);
			this.UnLoad.TabIndex = 3;
			this.UnLoad.Text = "Unload Animations";
			this.toolTip.SetToolTip(this.UnLoad, "Unload animation tags to load new tags");
			this.UnLoad.Click += new System.EventHandler(this.OnUnload);
			// 
			// InsertAnimation
			// 
			this.InsertAnimation.Enabled = false;
			this.InsertAnimation.Location = new System.Drawing.Point(192, 120);
			this.InsertAnimation.Name = "InsertAnimation";
			this.InsertAnimation.Size = new System.Drawing.Size(75, 80);
			this.InsertAnimation.TabIndex = 10;
			this.InsertAnimation.Text = "^    ^     Replace Animation";
			this.toolTip.SetToolTip(this.InsertAnimation, "Replace the current Target animation with the current Source Animation (adds oper" +
				"ation to queue list)");
			this.InsertAnimation.Click += new System.EventHandler(this.OnInsertAnimation);
			// 
			// TExtractAnimation
			// 
			this.TExtractAnimation.Enabled = false;
			this.TExtractAnimation.Location = new System.Drawing.Point(440, 120);
			this.TExtractAnimation.Name = "TExtractAnimation";
			this.TExtractAnimation.Size = new System.Drawing.Size(128, 23);
			this.TExtractAnimation.TabIndex = 8;
			this.TExtractAnimation.Text = "Extract Animation";
			this.toolTip.SetToolTip(this.TExtractAnimation, "Extract the Current Target\'s Animation Data");
			this.TExtractAnimation.Click += new System.EventHandler(this.OnTExtractAnimation);
			// 
			// SExtractAnimation
			// 
			this.SExtractAnimation.Enabled = false;
			this.SExtractAnimation.Location = new System.Drawing.Point(440, 224);
			this.SExtractAnimation.Name = "SExtractAnimation";
			this.SExtractAnimation.Size = new System.Drawing.Size(128, 23);
			this.SExtractAnimation.TabIndex = 15;
			this.SExtractAnimation.Text = "Extract Animation";
			this.toolTip.SetToolTip(this.SExtractAnimation, "Extract the Current Source\'s Animation Data");
			this.SExtractAnimation.Click += new System.EventHandler(this.OnSExtractAnimation);
			// 
			// SExtractFrameData
			// 
			this.SExtractFrameData.Enabled = false;
			this.SExtractFrameData.Location = new System.Drawing.Point(312, 224);
			this.SExtractFrameData.Name = "SExtractFrameData";
			this.SExtractFrameData.Size = new System.Drawing.Size(128, 23);
			this.SExtractFrameData.TabIndex = 14;
			this.SExtractFrameData.Text = "Extract Frame Data";
			this.toolTip.SetToolTip(this.SExtractFrameData, "Extract the Current Source\'s Animation\'s Frame Data");
			this.SExtractFrameData.Click += new System.EventHandler(this.OnSExtractFrameData);
			// 
			// SExtractDefaultData
			// 
			this.SExtractDefaultData.Enabled = false;
			this.SExtractDefaultData.Location = new System.Drawing.Point(440, 200);
			this.SExtractDefaultData.Name = "SExtractDefaultData";
			this.SExtractDefaultData.Size = new System.Drawing.Size(128, 23);
			this.SExtractDefaultData.TabIndex = 13;
			this.SExtractDefaultData.Text = "Extract Default Data";
			this.toolTip.SetToolTip(this.SExtractDefaultData, "Extract the Current Source\'s Animation\'s Default Data");
			this.SExtractDefaultData.Click += new System.EventHandler(this.OnSExtractDefaultData);
			// 
			// SExtractFrameInfo
			// 
			this.SExtractFrameInfo.Enabled = false;
			this.SExtractFrameInfo.Location = new System.Drawing.Point(312, 200);
			this.SExtractFrameInfo.Name = "SExtractFrameInfo";
			this.SExtractFrameInfo.Size = new System.Drawing.Size(128, 23);
			this.SExtractFrameInfo.TabIndex = 12;
			this.SExtractFrameInfo.Text = "Extract Frame Info";
			this.toolTip.SetToolTip(this.SExtractFrameInfo, "Extract the Current Source\'s Animation\'s Frame Info");
			this.SExtractFrameInfo.Click += new System.EventHandler(this.OnSExtractFrameInfo);
			// 
			// SExport
			// 
			this.SExport.Enabled = false;
			this.SExport.Location = new System.Drawing.Point(312, 248);
			this.SExport.Name = "SExport";
			this.SExport.Size = new System.Drawing.Size(256, 23);
			this.SExport.TabIndex = 16;
			this.SExport.Text = "Export Animation As...";
			this.toolTip.SetToolTip(this.SExport, "Export the Current Source\'s Animation to 3ds max Studio 5\\6");
			this.SExport.Click += new System.EventHandler(this.OnSExport);
			// 
			// TargetAnimationBlock
			// 
			this.TargetAnimationBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TargetAnimationBlock.Location = new System.Drawing.Point(136, 96);
			this.TargetAnimationBlock.Name = "TargetAnimationBlock";
			this.TargetAnimationBlock.Size = new System.Drawing.Size(176, 21);
			this.TargetAnimationBlock.TabIndex = 4;
			this.toolTip.SetToolTip(this.TargetAnimationBlock, "Current Target Animation");
			this.TargetAnimationBlock.MouseHover += new System.EventHandler(this.OnTargetMouseHover);
			// 
			// SourceAnimationBlock
			// 
			this.SourceAnimationBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SourceAnimationBlock.Location = new System.Drawing.Point(136, 200);
			this.SourceAnimationBlock.Name = "SourceAnimationBlock";
			this.SourceAnimationBlock.Size = new System.Drawing.Size(176, 21);
			this.SourceAnimationBlock.TabIndex = 11;
			this.toolTip.SetToolTip(this.SourceAnimationBlock, "Current Source Animation");
			this.SourceAnimationBlock.MouseHover += new System.EventHandler(this.OnSourceMouseHover);
			// 
			// ActionList
			// 
			this.ActionList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.SourceColumn,
																						 this.TargetColumn});
			this.ActionList.Enabled = false;
			this.ActionList.Location = new System.Drawing.Point(0, 320);
			this.ActionList.Name = "ActionList";
			this.ActionList.Size = new System.Drawing.Size(568, 97);
			this.ActionList.TabIndex = 17;
			this.toolTip.SetToolTip(this.ActionList, "Animation Operation Queue List");
			this.ActionList.View = System.Windows.Forms.View.Details;
			this.ActionList.DoubleClick += new System.EventHandler(this.OnActionListDoubleClick);
			// 
			// SourceColumn
			// 
			this.SourceColumn.Text = "Source Animation";
			this.SourceColumn.Width = 284;
			// 
			// TargetColumn
			// 
			this.TargetColumn.Text = "Replaced Animation";
			this.TargetColumn.Width = 270;
			// 
			// Compile
			// 
			this.Compile.Enabled = false;
			this.Compile.Location = new System.Drawing.Point(0, 448);
			this.Compile.Name = "Compile";
			this.Compile.Size = new System.Drawing.Size(576, 23);
			this.Compile.TabIndex = 18;
			this.Compile.Text = "Compile New Animations Tag";
			this.toolTip.SetToolTip(this.Compile, "Start the animation operation list. Output\'s new animation tag to the file you se" +
				"lected earlier");
			this.Compile.Click += new System.EventHandler(this.OnCompile);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(0, 296);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(304, 23);
			this.label3.TabIndex = 19;
			this.label3.Text = "Animation Operation Queue:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(0, 424);
			this.label4.Name = "label4";
			this.label4.TabIndex = 20;
			this.label4.Text = "Compiler Options:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FixNodes
			// 
			this.FixNodes.Location = new System.Drawing.Point(104, 424);
			this.FixNodes.Name = "FixNodes";
			this.FixNodes.Size = new System.Drawing.Size(136, 24);
			this.FixNodes.TabIndex = 21;
			this.FixNodes.Text = "Fix Node Checksums";
			// 
			// TargetBatchFrameInfo
			// 
			this.TargetBatchFrameInfo.Enabled = false;
			this.TargetBatchFrameInfo.Location = new System.Drawing.Point(0, 120);
			this.TargetBatchFrameInfo.Name = "TargetBatchFrameInfo";
			this.TargetBatchFrameInfo.Size = new System.Drawing.Size(136, 23);
			this.TargetBatchFrameInfo.TabIndex = 22;
			this.TargetBatchFrameInfo.Text = "Extract All FrameInfo";
			this.TargetBatchFrameInfo.Click += new System.EventHandler(this.OnTBExtractFrameInfo);
			// 
			// TargetBatchDefaultData
			// 
			this.TargetBatchDefaultData.Enabled = false;
			this.TargetBatchDefaultData.Location = new System.Drawing.Point(0, 144);
			this.TargetBatchDefaultData.Name = "TargetBatchDefaultData";
			this.TargetBatchDefaultData.Size = new System.Drawing.Size(136, 23);
			this.TargetBatchDefaultData.TabIndex = 23;
			this.TargetBatchDefaultData.Text = "Extract All DefaultData";
			this.TargetBatchDefaultData.Click += new System.EventHandler(this.OnTBExtractDefaultData);
			// 
			// TargetBatchFrameData
			// 
			this.TargetBatchFrameData.Enabled = false;
			this.TargetBatchFrameData.Location = new System.Drawing.Point(0, 168);
			this.TargetBatchFrameData.Name = "TargetBatchFrameData";
			this.TargetBatchFrameData.Size = new System.Drawing.Size(136, 23);
			this.TargetBatchFrameData.TabIndex = 24;
			this.TargetBatchFrameData.Text = "Extract All FrameData";
			this.TargetBatchFrameData.Click += new System.EventHandler(this.OnTBExtractFrameData);
			// 
			// SourceBatchFrameData
			// 
			this.SourceBatchFrameData.Enabled = false;
			this.SourceBatchFrameData.Location = new System.Drawing.Point(0, 272);
			this.SourceBatchFrameData.Name = "SourceBatchFrameData";
			this.SourceBatchFrameData.Size = new System.Drawing.Size(136, 23);
			this.SourceBatchFrameData.TabIndex = 27;
			this.SourceBatchFrameData.Text = "Extract All FrameData";
			this.SourceBatchFrameData.Click += new System.EventHandler(this.OnSBExtractFrameData);
			// 
			// SourceBatchDefaultData
			// 
			this.SourceBatchDefaultData.Enabled = false;
			this.SourceBatchDefaultData.Location = new System.Drawing.Point(0, 248);
			this.SourceBatchDefaultData.Name = "SourceBatchDefaultData";
			this.SourceBatchDefaultData.Size = new System.Drawing.Size(136, 23);
			this.SourceBatchDefaultData.TabIndex = 26;
			this.SourceBatchDefaultData.Text = "Extract All DefaultData";
			this.SourceBatchDefaultData.Click += new System.EventHandler(this.OnSBExtractDefaultData);
			// 
			// SourceBatchFrameInfo
			// 
			this.SourceBatchFrameInfo.Enabled = false;
			this.SourceBatchFrameInfo.Location = new System.Drawing.Point(0, 224);
			this.SourceBatchFrameInfo.Name = "SourceBatchFrameInfo";
			this.SourceBatchFrameInfo.Size = new System.Drawing.Size(136, 23);
			this.SourceBatchFrameInfo.TabIndex = 25;
			this.SourceBatchFrameInfo.Text = "Extract All FrameInfo";
			this.SourceBatchFrameInfo.Click += new System.EventHandler(this.OnSBExtractFrameInfo);
			// 
			// BatchExtract
			// 
			this.BatchExtract.SelectedPath = "C:\\Halo";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(576, 473);
			this.Controls.Add(this.SourceBatchFrameData);
			this.Controls.Add(this.SourceBatchDefaultData);
			this.Controls.Add(this.SourceBatchFrameInfo);
			this.Controls.Add(this.TargetBatchFrameData);
			this.Controls.Add(this.TargetBatchDefaultData);
			this.Controls.Add(this.TargetBatchFrameInfo);
			this.Controls.Add(this.FixNodes);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.Compile);
			this.Controls.Add(this.ActionList);
			this.Controls.Add(this.SourceAnimationBlock);
			this.Controls.Add(this.TargetAnimationBlock);
			this.Controls.Add(this.SExport);
			this.Controls.Add(this.SExtractAnimation);
			this.Controls.Add(this.SExtractFrameData);
			this.Controls.Add(this.SExtractDefaultData);
			this.Controls.Add(this.SExtractFrameInfo);
			this.Controls.Add(this.TExtractAnimation);
			this.Controls.Add(this.InsertAnimation);
			this.Controls.Add(this.UnLoad);
			this.Controls.Add(this.TExport);
			this.Controls.Add(this.TExtractFrameData);
			this.Controls.Add(this.TExtractDefaultData);
			this.Controls.Add(this.TExtractFrameInfo);
			this.Controls.Add(this.LoadButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.SourceTagRef);
			this.Controls.Add(this.TargetTagRef);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.Menu = this.MainMenu;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "HaloAnimationEditor";
			this.toolTip.SetToolTip(this, "Halo Animation Editor: by Kornman00");
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

		private void OnLoad(object sender, System.EventArgs e)
		{
			MessageBox.Show(this, "Select a file to save the new animations in", "Kornman00 says:");
			SaveFileDialog.Filter = "Model Animations|*.model_animations";

			if(SaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				output = new BinaryWriter(SaveFileDialog.OpenFile());

				#region GUI On
				TargetAnimationBlock.Enabled = true;
				SourceAnimationBlock.Enabled = true;
				LoadButton.Enabled = false;
				UnLoad.Enabled = true;
				InsertAnimation.Enabled = true;
				TargetTagRef.Enabled = false;
				SourceTagRef.Enabled = false;
				Compile.Enabled = true;
				ActionList.Enabled = true;

				// Target Buttons
				TExtractFrameInfo.Enabled = true;
				TExtractDefaultData.Enabled = true;
				TExtractFrameData.Enabled = true;
				TExtractAnimation.Enabled = true;
				TargetBatchFrameInfo.Enabled = true;
				TargetBatchDefaultData.Enabled = true;
				TargetBatchFrameData.Enabled = true;

				// Source Buttons
				SExtractFrameInfo.Enabled = true;
				SExtractDefaultData.Enabled = true;
				SExtractFrameData.Enabled = true;
				SExtractAnimation.Enabled = true;
				SourceBatchFrameInfo.Enabled = true;
				SourceBatchDefaultData.Enabled = true;
				SourceBatchFrameData.Enabled = true;
				#endregion

				// Do the source first, so our counters don't get fucked
				source = new BinaryReader(new StreamReader(this.SourceTagRef.Field).BaseStream); // Open Source Tag
				target = new BinaryReader(new StreamReader(this.TargetTagRef.Field).BaseStream); // Open Target Tag

				#region Source Anime
				SourceHeader = SkipAnimationBody(ref source);
				SourceAnimations = this.ReadAnimations(ref source);
				foreach(AnimationBlock a in SourceAnimations)
					SourceAnimationBlock.Items.Add(a.Name);
				SourceAnimationBlock.SelectedIndex = 0;
				#endregion

				#region Target Anime
				TargetHeader = SkipAnimationBody(ref target);
				TargetAnimations = this.ReadAnimations(ref target);
				foreach(AnimationBlock a in TargetAnimations)
					TargetAnimationBlock.Items.Add(a.Name);
				TargetAnimationBlock.SelectedIndex = 0;

				output.Write(TargetHeader); output.Flush();
				#endregion

				if(TargetAnimations[0] != null && SourceAnimations[0] != null)
				{
					if(TargetAnimations[0].nodeChecksum != SourceAnimations[0].nodeChecksum)
					{
						if(MessageBox.Show(this, "The animation's node checksum do not match! Continue?", "Kornman00 says:", MessageBoxButtons.YesNo) == DialogResult.No)
						{
							OnUnload(null, null);
							return;
						}
					}
				}
			}
		}

		private void OnUnload(object sender, System.EventArgs e)
		{
			#region GUI Off
			this.TargetAnimationBlock.Enabled = false;
			this.SourceAnimationBlock.Enabled = false;
			this.LoadButton.Enabled = true;
			this.UnLoad.Enabled = false;
			this.InsertAnimation.Enabled = false;
			this.TargetTagRef.Enabled = true;
			this.SourceTagRef.Enabled = true;
			ActionList.Enabled = false;
			Compile.Enabled = false;

			// Target Buttons
			this.TExtractFrameInfo.Enabled = false;
			this.TExtractDefaultData.Enabled = false;
			this.TExtractFrameData.Enabled = false;
			this.TExtractAnimation.Enabled = false;
			TargetBatchFrameInfo.Enabled = false;
			TargetBatchDefaultData.Enabled = false;
			TargetBatchFrameData.Enabled = false;

			// Source Buttons
			this.SExtractFrameInfo.Enabled = false;
			this.SExtractDefaultData.Enabled = false;
			this.SExtractFrameData.Enabled = false;
			this.SExtractAnimation.Enabled = false;
			SourceBatchFrameInfo.Enabled = false;
			SourceBatchDefaultData.Enabled = false;
			SourceBatchFrameData.Enabled = false;

			this.TargetAnimationBlock.Items.Clear();
			this.SourceAnimationBlock.Items.Clear();
			ActionList.Items.Clear();
			#endregion

			this.ResetCounters();

			// Close our Streams
			if(target != null)
				target.Close();
			if(source != null)
				source.Close();
			if(output != null)
				output.Close();
		}

		private void OnInsertAnimation(object sender, System.EventArgs e)
		{
			// Add the source animation to the list...
			ListViewItem source = new ListViewItem(SourceAnimations[SourceAnimationBlock.SelectedIndex].Name);
			source.Tag = SourceAnimationBlock.SelectedIndex;

			//...then add the target animation to the next column, to the right
			source.SubItems.Add(string.Format("{0}", TargetAnimations[TargetAnimationBlock.SelectedIndex].Name));

			// Add them both to the list
			ActionList.Items.Add(source);
		}

		private void OnCompile(object sender, System.EventArgs e)
		{
			int[] TargetIndexs = new int[ActionList.Items.Count];
			int[] SourceIndexs = new int[ActionList.Items.Count];

			int x = 0;
			foreach(ListViewItem lvi in ActionList.Items)
			{
				SourceIndexs[x] = (int)lvi.Tag;
				TargetIndexs[x] = FindAnimation(lvi.SubItems[1].Text, TargetAnimations);

				x++;
			}

			for(x = 0; x < ActionList.Items.Count; x++)
				if(FixNodes.Checked == true)
					SourceAnimations[SourceIndexs[x]].nodeChecksum = TargetAnimations[TargetIndexs[x]].nodeChecksum;

			for(x = 0; x < ActionList.Items.Count; x++)
				TargetAnimations[TargetIndexs[x]] = SourceAnimations[SourceIndexs[x]];

			for(x = 0; x < AnimationsCount; x++)
				TargetAnimations[x].WriteMeta(ref output);

			for(x = 0; x < AnimationsCount; x++)
				TargetAnimations[x].WriteData(ref output);

			output.Flush();

			MessageBox.Show(this, "Done Compiling! You can now open in Guerilla (or Kornman00.exe) or use in game.", "Kornman00 says:");
			MessageBox.Show(this, "O, and BTW: Halo2 Mod Team Pwns j00!", "Kornman00 says:");
		}

		private void OnActionListDoubleClick(object sender, System.EventArgs e)
		{
			foreach(ListViewItem lvi in ActionList.SelectedItems)
				ActionList.Items.Remove(lvi);
		}

		
		#region Target Buttons
		private void OnTargetMouseHover(object sender, System.EventArgs e)
		{
			if(TargetAnimationBlock.Enabled != false && TargetAnimations != null)
			{
				toolTip.SetToolTip(TargetAnimationBlock, string.Format("Animation Name: {0}\n" +
					"Frame Info Size: {1}\n" +
					"Default Data Size: {2}\n" +
					"Frame Data Size: {3}\n" +
					"Node Checksum: {4}",
					TargetAnimations[TargetAnimationBlock.SelectedIndex].Name,
					TargetAnimations[TargetAnimationBlock.SelectedIndex].FrameInfo,
					TargetAnimations[TargetAnimationBlock.SelectedIndex].DefaultData,
					TargetAnimations[TargetAnimationBlock.SelectedIndex].FrameData,
					TargetAnimations[TargetAnimationBlock.SelectedIndex].nodeChecksum));
			}
		}

		private void OnTExtractFrameInfo(object sender, System.EventArgs e)
		{
			ExtractDialog.Filter = "Frame Info|*.frame_info";
			ExtractDialog.Title = "Extract Frame Info";
			ExtractDialog.FileName = TargetAnimationBlock.SelectedText;
			if(ExtractDialog.ShowDialog() == DialogResult.OK)
			{
				AnimationBlock CurrentTarget = TargetAnimations[TargetAnimationBlock.SelectedIndex];
				CurrentTarget.WriteFrameInfo(new BinaryWriter(new StreamWriter(ExtractDialog.FileName).BaseStream));
			}		
		}

		private void OnTBExtractFrameInfo(object sender, System.EventArgs e)
		{
			if(BatchExtract.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw;
				BinaryWriter bw;
				for(int x = 0; x < TargetAnimations.Length; x++)
				{
					if(TargetAnimations[x].FrameInfo != 0)
					{
						sw = new StreamWriter(BatchExtract.SelectedPath + "\\" + TargetAnimations[x].Name + ".frame_info");
						bw = new BinaryWriter(sw.BaseStream);
						TargetAnimations[x].WriteFrameInfo(ref bw);
						bw.Close();
					}
				}
			}
		}

		private void OnTExtractDefaultData(object sender, System.EventArgs e)
		{
			ExtractDialog.Filter = "Default Data|*.default_data";
			ExtractDialog.Title = "Extract Default Data";
			if(ExtractDialog.ShowDialog() == DialogResult.OK)
			{
				AnimationBlock CurrentTarget = TargetAnimations[TargetAnimationBlock.SelectedIndex];
				CurrentTarget.WriteDefaultData(new BinaryWriter(new StreamWriter(ExtractDialog.FileName).BaseStream));
			}		
		}

		private void OnTBExtractDefaultData(object sender, System.EventArgs e)
		{
			if(BatchExtract.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw;
				BinaryWriter bw;
				for(int x = 0; x < TargetAnimations.Length; x++)
				{
					if(TargetAnimations[x].DefaultData != 0)
					{
						sw = new StreamWriter(BatchExtract.SelectedPath + "\\" + TargetAnimations[x].Name + ".default_data");
						bw = new BinaryWriter(sw.BaseStream);
						TargetAnimations[x].WriteDefaultData(ref bw);
						bw.Close();
					}
				}
			}
		}

		private void OnTExtractFrameData(object sender, System.EventArgs e)
		{
			ExtractDialog.Filter = "Frame Data|*.frame_data";
			ExtractDialog.Title = "Extract Frame Data";
			if(ExtractDialog.ShowDialog() == DialogResult.OK)
			{
				AnimationBlock CurrentTarget = TargetAnimations[TargetAnimationBlock.SelectedIndex];
				CurrentTarget.WriteFrameData(new BinaryWriter(new StreamWriter(ExtractDialog.FileName).BaseStream));
			}		
		}

		private void OnTBExtractFrameData(object sender, System.EventArgs e)
		{
			if(BatchExtract.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw;
				BinaryWriter bw;
				for(int x = 0; x < TargetAnimations.Length; x++)
				{
					if(TargetAnimations[x].FrameData != 0)
					{
						sw = new StreamWriter(BatchExtract.SelectedPath + "\\" + TargetAnimations[x].Name + ".frame_data");
						bw = new BinaryWriter(sw.BaseStream);
						TargetAnimations[x].WriteFrameData(ref bw);
						bw.Close();
					}
				}
			}
		}

		private void OnTExtractAnimation(object sender, System.EventArgs e)
		{
			ExtractDialog.Filter = "Single Animation|*.single_animation";
			ExtractDialog.Title = "Extract Animation";
			if(ExtractDialog.ShowDialog() == DialogResult.OK)
			{
				AnimationBlock CurrentTarget = TargetAnimations[TargetAnimationBlock.SelectedIndex];
				BinaryWriter bw = new BinaryWriter(new StreamWriter(ExtractDialog.FileName).BaseStream);
				CurrentTarget.WriteData(ref bw);
			}		
		}

		private void OnTExport(object sender, System.EventArgs e)
		{
			ExtractDialog.Filter = "Max6 Scene|*.max";
			ExtractDialog.Title = "Export Animation to...";
			if(ExtractDialog.ShowDialog() == DialogResult.OK)
			{
				AnimationBlock CurrentTarget = TargetAnimations[TargetAnimationBlock.SelectedIndex];
			}		
		}
		#endregion Target Buttons

		#region Source Buttons
		private void OnSourceMouseHover(object sender, System.EventArgs e)
		{
			if(SourceAnimationBlock.Enabled != false && SourceAnimations != null)
			{
				toolTip.SetToolTip(SourceAnimationBlock, string.Format("Animation Name: {0}\n" +
					"Frame Info Size: {1}\n" +
					"Default Data Size: {2}\n" +
					"Frame Data Size: {3}\n" +
					"Node Checksum: {4}",
					SourceAnimations[SourceAnimationBlock.SelectedIndex].Name,
					SourceAnimations[SourceAnimationBlock.SelectedIndex].FrameInfo,
					SourceAnimations[SourceAnimationBlock.SelectedIndex].DefaultData,
					SourceAnimations[SourceAnimationBlock.SelectedIndex].FrameData,
					SourceAnimations[SourceAnimationBlock.SelectedIndex].nodeChecksum));
			}
		}

		private void OnSExtractFrameInfo(object sender, System.EventArgs e)
		{
			ExtractDialog.Filter = "Frame Info|*.frame_info";
			ExtractDialog.Title = "Extract Frame Info";
			if(ExtractDialog.ShowDialog() == DialogResult.OK)
			{
				AnimationBlock CurrentSource = SourceAnimations[SourceAnimationBlock.SelectedIndex];
				CurrentSource.WriteFrameInfo(new BinaryWriter(new StreamWriter(ExtractDialog.FileName).BaseStream));
			}
		}

		private void OnSBExtractFrameInfo(object sender, System.EventArgs e)
		{
			if(BatchExtract.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw;
				BinaryWriter bw;
				for(int x = 0; x < SourceAnimations.Length; x++)
				{
					if(SourceAnimations[x].FrameInfo != 0)
					{
						sw = new StreamWriter(BatchExtract.SelectedPath + "\\" + SourceAnimations[x].Name + ".frame_info");
						bw = new BinaryWriter(sw.BaseStream);
						SourceAnimations[x].WriteFrameInfo(ref bw);
						bw.Close();
					}
				}
			}
		}

		private void OnSExtractDefaultData(object sender, System.EventArgs e)
		{
			ExtractDialog.Filter = "Default Data|*.default_data";
			ExtractDialog.Title = "Extract Default Data";
			if(ExtractDialog.ShowDialog() == DialogResult.OK)
			{
				AnimationBlock CurrentSource = SourceAnimations[SourceAnimationBlock.SelectedIndex];
				CurrentSource.WriteDefaultData(new BinaryWriter(new StreamWriter(ExtractDialog.FileName).BaseStream));
			}		
		}

		private void OnSBExtractDefaultData(object sender, System.EventArgs e)
		{
			if(BatchExtract.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw;
				BinaryWriter bw;
				for(int x = 0; x < SourceAnimations.Length; x++)
				{
					if(SourceAnimations[x].DefaultData != 0)
					{
						sw = new StreamWriter(BatchExtract.SelectedPath + "\\" + SourceAnimations[x].Name + ".default_data");
						bw = new BinaryWriter(sw.BaseStream);
						SourceAnimations[x].WriteDefaultData(ref bw);
						bw.Close();
					}
				}
			}
		}

		private void OnSExtractFrameData(object sender, System.EventArgs e)
		{
			ExtractDialog.Filter = "Frame Data|*.frame_data";
			ExtractDialog.Title = "Extract Frame Data";
			if(ExtractDialog.ShowDialog() == DialogResult.OK)
			{
				AnimationBlock CurrentSource = SourceAnimations[SourceAnimationBlock.SelectedIndex];
				CurrentSource.WriteFrameData(new BinaryWriter(new StreamWriter(ExtractDialog.FileName).BaseStream));
			}		
		}

		private void OnSBExtractFrameData(object sender, System.EventArgs e)
		{
			if(BatchExtract.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw;
				BinaryWriter bw;
				for(int x = 0; x < SourceAnimations.Length; x++)
				{
					if(SourceAnimations[x].FrameData != 0)
					{
						sw = new StreamWriter(BatchExtract.SelectedPath + "\\" + SourceAnimations[x].Name + ".frame_data");
						bw = new BinaryWriter(sw.BaseStream);
						SourceAnimations[x].WriteFrameData(ref bw);
						bw.Close();
					}
				}
			}
		}

		private void OnSExtractAnimation(object sender, System.EventArgs e)
		{
			ExtractDialog.Filter = "Single Animation|*.single_animation";
			ExtractDialog.Title = "Extract Animation";
			if(ExtractDialog.ShowDialog() == DialogResult.OK)
			{
				AnimationBlock CurrentSource = SourceAnimations[SourceAnimationBlock.SelectedIndex];
				BinaryWriter bw = new BinaryWriter(new StreamWriter(ExtractDialog.FileName).BaseStream);
				CurrentSource.WriteData(ref bw);
			}		
		}

		private void OnSExport(object sender, System.EventArgs e)
		{
			ExtractDialog.Filter = "Max6 Scene|*.max";
			ExtractDialog.Title = "Export Animation to...";
			if(ExtractDialog.ShowDialog() == DialogResult.OK)
			{
				AnimationBlock CurrentSource = SourceAnimations[SourceAnimationBlock.SelectedIndex];
			}		
		}
		#endregion Source Buttons

		private void OnAbout(object sender, System.EventArgs e)
		{
			About a = new About();
			a.ShowDialog(this);
		}

		private void OnExit(object sender, System.EventArgs e)
		{
			OnUnload(null, null);
			this.Close();
		}

	}
}