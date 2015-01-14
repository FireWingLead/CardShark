using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardShark.PCShark.DAL.SQLiteLocal
{
	public enum ManaColors
	{
		Colorless = 1,
		White = 2,
		Blue = 4,
		Black = 8,
		Red = 16,
		Green = 32,
		Phyrexian = 64,
		Snow = 128,
		Half = 256,

		WhiteBlue = White | Blue,
		WhiteBlack = White | Black,
		BlueBlack = Blue | Black,
		BlueRed = Blue | Red,
		BlackRed = Black | Red,
		BlackGreen = Black | Green,
		RedGreen = Red | Green,
		RedWhite = Red | White,
		GreenWhite = Green | White,
		GreenBlue = Green | Blue,

		WhiteBlueBlack = White | Blue | Black,
		WhiteBlueRed = White | Blue | Red,
		WhiteBlueGreen = White | Blue | Green,
		WhiteBlackRed = White | Black | Red,
		WhiteBlackGreen = White | Black | Green,
		WhiteRedGreen = White | Red | Green,
		BlueBlackRed = Blue | Black | Red,
		BlueBlackGreen = Blue | Black | Green,
		BlueRedGreen = Blue | Red | Green,
		BlackRedGreen = Black | Red | Green,

		WhiteBlueBlackRed = White | Blue | Black | Red,
		WhiteBlueBlackGreen = White | Blue | Black | Green,
		WhiteBlueRedGreen = White | Blue | Red | Green,
		BlueBlackRedGreen = Blue | Black | Red | Green,

		WhiteBlueBlackRedGreen = White | Blue | Black | Red | Green,

		PhyrexianWhite = Phyrexian | White,
		PhyrexianBlue = Phyrexian | Blue,
		PhyrexianBlack = Phyrexian | Black,
		PhyrexianRed = Phyrexian | Red,
		PhyrexianGreen = Phyrexian | Green,

		ColorlessOrWhite = Colorless|White,
		ColorlessOrBlue = Colorless|Blue,
		ColorlessOrBlack = Colorless|Black,
		ColorlessOrRed = Colorless|Red,
		ColorlessOrGreen = Colorless|Green
	}
}
