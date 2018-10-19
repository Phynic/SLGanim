namespace Sirenix.OdinInspector.Demos
{
	using System;
	using UnityEngine;
	using Sirenix.Utilities;

#if UNITY_EDITOR
	using Sirenix.Utilities.Editor;
	using Sirenix.OdinInspector.Editor;
	using UnityEditor;
#endif

	public class MinesweeperExample : MonoBehaviour
	{
		[Minesweeper]
		public int NumberOfBombs;
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class MinesweeperAttribute : Attribute
	{ }

#if UNITY_EDITOR

	/// <summary>
	/// Minesweeper.
	/// </summary>
	[OdinDrawer]
	public sealed class MinesweeperAttributeDrawer : OdinAttributeDrawer<MinesweeperAttribute, int>
	{
		private enum Tile
		{
			None = 0, // Empty tile.

			// 1-8.

			Open = 9,
			Bomb = 10,
			Flag = 11,
		}

		private readonly Color[] NumberColors = new Color[8]
		{
			new Color32(42, 135, 238, 255),		// 1
			new Color32(57, 233, 48, 255),		// 2
			new Color32(253, 0, 0, 255),		// 3
			new Color32(31, 23, 173, 255),		// 4
			new Color32(36, 30, 155, 255),		// 5
			new Color32(131, 29, 29, 255),		// 6
			new Color32(40, 40, 40, 255),		// 7
			new Color32(132, 132, 132, 255),    // 8
		};

		private const float TileSize = 20;
		private const int BoardSize = 25;

		private class GameContext
		{
			public bool IsRunning = false;
			public bool GameOver = false;
			public int FlaggedBombs = 0;
			public int NumberOfBombs;
			public Tile[,] VisibleTiles;
			public Tile[,] Tiles;
			public double Time;
			public double PrevTime;
			public object Key;
		}

		/// <summary>
		/// Handles the Minesweeper game.
		/// </summary>
		protected override void DrawPropertyLayout(IPropertyValueEntry<int> entry, MinesweeperAttribute attribute, GUIContent label)
		{
			PropertyContext<GameContext> context;
			if (entry.Context.Get(this, "GameContext", out context))
			{
				context.Value = new GameContext()
				{
					IsRunning = false,
					GameOver = false,
					VisibleTiles = new Tile[BoardSize, BoardSize],
					Tiles = new Tile[BoardSize, BoardSize],
					Key = new object(),
				};
			}

			GameContext game = context.Value;

			Rect rect = EditorGUILayout.GetControlRect();
			entry.SmartValue = Mathf.Clamp(SirenixEditorFields.IntField(rect.AlignLeft(rect.width - 80 - 4), "Number of Bombs", entry.SmartValue), 1, (BoardSize * BoardSize) / 4);

			// Start game
			if (GUI.Button(rect.AlignRight(80), "Start"))
			{
				game.NumberOfBombs = entry.SmartValue;
				game.FlaggedBombs = 0;

				for (int x = 0; x < BoardSize; x++)
				{
					for (int y = 0; y < BoardSize; y++)
					{
						game.VisibleTiles[x, y] = Tile.None;
						game.Tiles[x, y] = Tile.None;
					}
				}

				// Spawn bombs.
				for (int count = 0; count < game.NumberOfBombs;)
				{
					int x = UnityEngine.Random.Range(0, BoardSize);
					int y = UnityEngine.Random.Range(0, BoardSize);

					if (game.Tiles[x, y] != Tile.Bomb)
					{
						game.Tiles[x, y] = Tile.Bomb;

						if (x + 1 < BoardSize && game.Tiles[x + 1, y] != Tile.Bomb)
						{
							game.Tiles[x + 1, y] = (Tile)((int)game.Tiles[x + 1, y] + 1);
						}
						if (x + 1 < BoardSize && y + 1 < BoardSize && game.Tiles[x + 1, y + 1] != Tile.Bomb)
						{
							game.Tiles[x + 1, y + 1] = (Tile)((int)game.Tiles[x + 1, y + 1] + 1);
						}
						if (y + 1 < BoardSize && game.Tiles[x, y + 1] != Tile.Bomb)
						{
							game.Tiles[x, y + 1] = (Tile)((int)game.Tiles[x, y + 1] + 1);
						}
						if (x - 1 >= 0 && y + 1 < BoardSize && game.Tiles[x - 1, y + 1] != Tile.Bomb)
						{
							game.Tiles[x - 1, y + 1] = (Tile)((int)game.Tiles[x - 1, y + 1] + 1);
						}

						if (x - 1 >= 0 && game.Tiles[x - 1, y] != Tile.Bomb)
						{
							game.Tiles[x - 1, y] = (Tile)((int)game.Tiles[x - 1, y] + 1);
						}
						if (x - 1 >= 0 && y - 1 >= 0 && game.Tiles[x - 1, y - 1] != Tile.Bomb)
						{
							game.Tiles[x - 1, y - 1] = (Tile)((int)game.Tiles[x - 1, y - 1] + 1);
						}
						if (y - 1 >= 0 && game.Tiles[x, y - 1] != Tile.Bomb)
						{
							game.Tiles[x, y - 1] = (Tile)((int)game.Tiles[x, y - 1] + 1);
						}
						if (x + 1 < BoardSize && y - 1 >= 0 && game.Tiles[x + 1, y - 1] != Tile.Bomb)
						{
							game.Tiles[x + 1, y - 1] = (Tile)((int)game.Tiles[x + 1, y - 1] + 1);
						}

						count++;
					}
				}

				game.IsRunning = true;
				game.GameOver = false;
				game.PrevTime = EditorApplication.timeSinceStartup;
				game.Time = 0;
			}

			// Game
			SirenixEditorGUI.BeginShakeableGroup(game.Key);
			if (game.IsRunning)
			{
				this.Game(game);
			}
			SirenixEditorGUI.EndShakeableGroup(game.Key);
		}

		private void Game(GameContext game)
		{
			Rect rect = EditorGUILayout.GetControlRect(true, TileSize * BoardSize + 20);
			rect = rect.AlignCenter(TileSize * BoardSize);

			// Toolbar
			{
				SirenixEditorGUI.DrawSolidRect(rect.AlignTop(20), new Color(0.5f, 0.5f, 0.5f, 1f));
				SirenixEditorGUI.DrawBorders(rect.AlignTop(20).SetHeight(21).SetWidth(rect.width + 1), 1);

				if (Event.current.type == EventType.Repaint && !game.GameOver)
				{
					double t = EditorApplication.timeSinceStartup;
					game.Time += t - game.PrevTime;
					game.PrevTime = t;
				}

				var time = GUIHelper.TempContent(((int)game.Time).ToString());
				GUIHelper.PushContentColor(Color.black);
				GUI.Label(rect.AlignTop(20).HorizontalPadding(4).AlignMiddle(18).AlignRight(EditorStyles.label.CalcSize(time).x), time);
				GUIHelper.PopContentColor();

				GUI.Label(rect.AlignTop(20).AlignCenter(20), EditorIcons.PacmanGhost.ActiveGUIContent);

				if (game.GameOver)
				{
					GUIHelper.PushContentColor(game.FlaggedBombs == game.NumberOfBombs ? Color.green : Color.red);
					GUI.Label(rect.AlignTop(20).HorizontalPadding(4).AlignMiddle(18), game.FlaggedBombs == game.NumberOfBombs ? "You win!" : "Game over!");
					GUIHelper.PopContentColor();
				}
			}

			rect = rect.AlignBottom(rect.height - 20);
			SirenixEditorGUI.DrawSolidRect(rect, new Color(0.7f, 0.7f, 0.7f, 1f));

			for (int i = 0; i < BoardSize * BoardSize; i++)
			{
				Rect tileRect = rect.SplitGrid(TileSize, TileSize, i);
				SirenixEditorGUI.DrawBorders(tileRect.SetWidth(tileRect.width + 1).SetHeight(tileRect.height + 1), 1);

				int x = i % BoardSize;
				int y = i / BoardSize;
				var tile = game.Tiles[x, y];
				var visible = game.VisibleTiles[x, y];

				if (game.GameOver || visible == Tile.Open)
				{
					SirenixEditorGUI.DrawSolidRect(new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1), new Color(0.3f, 0.3f, 0.3f, 1f));
				}

				if ((game.GameOver || visible == Tile.Open) && tile == Tile.Bomb)
				{
					GUIHelper.PushColor(visible == Tile.Flag ? Color.black : Color.white);
					GUI.Label(tileRect.AlignCenter(18).AlignMiddle(18), EditorIcons.SettingsCog.ActiveGUIContent);
					GUIHelper.PopColor();
				}

				if (visible == Tile.Flag)
				{
					GUIHelper.PushColor(Color.red);
					GUI.Label(tileRect.AlignCenter(18).AlignMiddle(18), EditorIcons.Flag.ActiveGUIContent);
					GUIHelper.PopColor();
				}

				if ((game.GameOver || visible == Tile.Open) && (int)tile >= 1 && (int)tile <= 8)
				{
					GUIHelper.PushColor(this.NumberColors[(int)tile - 1]);
					GUI.Label(tileRect.AlignCenter(18).AlignCenter(18).AddX(2).AddY(2), ((int)tile).ToString(), EditorStyles.boldLabel);
					GUIHelper.PopColor();
				}

				if (!game.GameOver && tileRect.Contains(Event.current.mousePosition))
				{
					SirenixEditorGUI.DrawSolidRect(new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1), new Color(0f, 1f, 0f, 0.3f));

					// Input
					// Reveal
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
					{
						if (visible != Tile.Flag)
						{
							if (tile == Tile.Bomb)
							{
								// LOSE
								game.GameOver = true;
								SirenixEditorGUI.StartShakingGroup(game.Key, 3f);
							}
							else
							{
								this.Reveal(game, x, y);
							}
						}

						Event.current.Use();
					}
					// Place flag
					else if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
					{
						if (visible == Tile.None)
						{
							game.VisibleTiles[x, y] = Tile.Flag;

							if (tile == Tile.Bomb)
							{
								game.FlaggedBombs++;

								if (game.FlaggedBombs == game.NumberOfBombs)
								{
									game.GameOver = true;
								}
							}
						}
						else if (visible == Tile.Flag)
						{
							game.VisibleTiles[x, y] = Tile.None;

							if (tile == Tile.Bomb)
							{
								game.FlaggedBombs--;
							}
						}

						Event.current.Use();
					}
				}
			}

			GUIHelper.RequestRepaint();
		}

		private void Reveal(GameContext game, int x, int y)
		{
			if (x < 0 || x >= BoardSize || y < 0 || y >= BoardSize)
			{
				return;
			}

			if (game.VisibleTiles[x, y] == Tile.Open)
			{
				return;
			}

			if (game.Tiles[x, y] == Tile.Bomb)
			{
				return;
			}

			if ((int)game.Tiles[x, y] <= 8)
			{
				game.VisibleTiles[x, y] = Tile.Open;

				if (game.Tiles[x, y] != Tile.None)
				{
					return;
				}
			}

			// Recursive reveal.
			this.Reveal(game, x + 1, y);
			this.Reveal(game, x + 1, y + 1);
			this.Reveal(game, x, y + 1);
			this.Reveal(game, x - 1, y + 1);

			this.Reveal(game, x - 1, y);
			this.Reveal(game, x - 1, y - 1);
			this.Reveal(game, x, y - 1);
			this.Reveal(game, x + 1, y - 1);
		}
	}

#endif
}