﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.DanganRonpa.Nonstop;
using HyoutaTools.DanganRonpa.Lin;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.DanganRonpa.NonstopExistingDatabaseImport {
	class Importer {

		public static Dictionary<string, string> nonstopDict = new Dictionary<string, string> { { "nonstop_01_001.dat", "e01_102_001.lin" }, { "nonstop_01_002.dat", "e01_104_001.lin" }, { "nonstop_01_003.dat", "e01_106_001.lin" }, { "nonstop_01_004.dat", "e01_110_001.lin" }, { "nonstop_01_005.dat", "e01_112_001.lin" }, { "nonstop_01_006.dat", "e01_114_001.lin" }, { "nonstop_01_007.dat", "e01_116_001.lin" }, { "nonstop_01_008.dat", "e01_118_001.lin" }, { "nonstop_02_001.dat", "e02_102_001.lin" }, { "nonstop_02_002.dat", "e02_104_001.lin" }, { "nonstop_02_003.dat", "e02_108_001.lin" }, { "nonstop_02_004.dat", "e02_110_001.lin" }, { "nonstop_02_005.dat", "e02_112_001.lin" }, { "nonstop_02_006.dat", "e02_114_001.lin" }, { "nonstop_02_007.dat", "e02_118_001.lin" }, { "nonstop_02_008.dat", "e02_120_001.lin" }, { "nonstop_02_009.dat", "e02_122_001.lin" }, { "nonstop_03_001.dat", "e03_102_001.lin" }, { "nonstop_03_002.dat", "e03_106_001.lin" }, { "nonstop_03_003.dat", "e03_108_001.lin" }, { "nonstop_03_004.dat", "e03_112_001.lin" }, { "nonstop_03_005.dat", "e03_114_001.lin" }, { "nonstop_03_006.dat", "e03_116_001.lin" }, { "nonstop_03_007.dat", "e03_118_001.lin" }, { "nonstop_03_008.dat", "e03_120_001.lin" }, { "nonstop_03_009.dat", "e03_122_001.lin" }, { "nonstop_03_010.dat", "e03_124_001.lin" }, { "nonstop_03_011.dat", "e03_126_001.lin" }, { "nonstop_04_001.dat", "e04_102_001.lin" }, { "nonstop_04_002.dat", "e04_104_001.lin" }, { "nonstop_04_003.dat", "e04_106_001.lin" }, { "nonstop_04_004.dat", "e04_110_001.lin" }, { "nonstop_04_005.dat", "e04_116_001.lin" }, { "nonstop_04_006.dat", "e04_118_001.lin" }, { "nonstop_04_007.dat", "e04_122_001.lin" }, { "nonstop_04_008.dat", "e04_124_001.lin" }, { "nonstop_05_001.dat", "e05_102_001.lin" }, { "nonstop_05_002.dat", "e05_104_001.lin" }, { "nonstop_05_003.dat", "e05_110_001.lin" }, { "nonstop_05_004.dat", "e05_114_001.lin" }, { "nonstop_05_005.dat", "e05_116_001.lin" }, { "nonstop_05_006.dat", "e05_118_001.lin" }, { "nonstop_05_007.dat", "e05_120_001.lin" }, { "nonstop_05_008.dat", "e05_122_001.lin" }, { "nonstop_05_009.dat", "e05_151_001.lin" }, { "nonstop_06_001.dat", "e06_102_001.lin" }, { "nonstop_06_002.dat", "e06_106_001.lin" }, { "nonstop_06_003.dat", "e06_108_001.lin" }, { "nonstop_06_004.dat", "e06_110_001.lin" }, { "nonstop_06_005.dat", "e06_112_001.lin" }, { "nonstop_06_006.dat", "e06_118_001.lin" }, { "nonstop_06_007.dat", "e06_120_001.lin" }, { "nonstop_06_008.dat", "e06_134_001.lin" }, { "nonstop_06_009.dat", "e06_137_001.lin" } };
		public static int Import( string[] args ) {
			//if ( args.Length < 1 ) {
			//    Console.WriteLine( "Usage: text.lin.orig text.lin.new database [alignment (default 1024)]" );
			//    return -1;
			//}

			//int Alignment;
			//if ( !( args.Length >= 4 && Int32.TryParse( args[3], out Alignment ) ) ) {
			//    Alignment = 1024;
			//}
			//return LinExport.Exporter.Export( args[0], args[1], args[2], Alignment );
			return -1;
		}

		public static string GetFromSubstring( string[] strs, string sub ) {
			foreach ( string s in strs ) { if ( s.Contains( sub ) ) return s; }
			return null;
		}
		public static int AutoImport() {
			string dir = @"d:\_svn\GraceNote\GraceNote\DanganRonpaBestOfRebuild\umdimage.dat.ex\";
			string[] files = System.IO.Directory.GetFiles( dir );

			foreach ( var x in nonstopDict ) {
				string nonstopFile = GetFromSubstring( files, x.Key );
				string scriptFile = GetFromSubstring( files, x.Value );
				string databaseId = scriptFile.Substring( 0, 4 );
				string databaseFile = @"d:\_svn\GraceNote\GraceNote\DanganRonpaBestOfDB\DRBO" + databaseId;

				LIN lin = new LIN( scriptFile );
				Nonstop nonstop = new Nonstop( nonstopFile );

				int lastScriptEntry = 0;
				foreach ( var item in nonstop.items ) {
					int stringId = item.data[(int)NonstopSingleStructure.StringID];
					int correspondingTextEntry = stringId * 2;
					int correspondingScriptEntry = correspondingTextEntry - 1;
					if ( item.data[(int)NonstopSingleStructure.Type] == 0 ) {
						lastScriptEntry = correspondingScriptEntry;
					}

					var text = new SQLiteParameter();
					var id = new SQLiteParameter();
					id.Value = correspondingScriptEntry;
					 GenericSqliteUpdate(
						 "Data Source=" + databaseFile,
						 "UPDATE Text SET comment = comment || ? WHERE stringid = ?",
						 new SQLiteParameter[]{ text, id } );

				}
			}

			return 0;
		}

		public static int GenericSqliteUpdate( string connString, string statement ) {
			return GenericSqliteUpdate( connString, statement, new SQLiteParameter[0] );
		}
		public static int GenericSqliteUpdate( string connString, string statement, IEnumerable<SQLiteParameter> parameters ) {
			int affected = -1;
			SQLiteConnection Connection = new SQLiteConnection( connString );
			Connection.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = statement;
				foreach ( SQLiteParameter p in parameters ) {
					Command.Parameters.Add( p );
				}
				affected = Command.ExecuteNonQuery();
				Transaction.Commit();
			}
			Connection.Close();

			return affected;
		}
	}
}