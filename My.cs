using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public class My
	{
		Hockeyist self;
		World world;
		Game game;
		Move move;
		public My (Hockeyist _self, World _world, Game _game, Move _move)
		{
		self = _self;
		world = _world;
		game = _game;
		move = _move;
		}
	}
}

