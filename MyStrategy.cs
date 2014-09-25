using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk {
    public sealed class MyStrategy : IStrategy {
		private ArrayList args = new ArrayList();
		private Hockeyist self;
		private World world;
		private Game game;
		private Move move;
		private double[] AimGoalPoint = new double[2];
		private enum HockStateType {none, goingToAim, aiming};
		private HockStateType HockState = HockStateType.none;
		//private Hockeyist[] opponentMinions;
		private Player opponent;
		//private double radius1 = 1D;
						
        public void Move(Hockeyist _self, World _world, Game _game, Move _move) {
			args.Add(_self); args.Add(_world); args.Add(_game); args.Add(_move);
			Init();
			
			//opponentMinions = AllBadGuys();					
			opponent = world.GetOpponentPlayer();
			//if (opponent.IsJustMissedGoal||opponent.IsJustScoredGoal) return;
			AimGoalPoint[0] = opponent.NetFront + world.Width/6 + self.Radius;
			if (self.Y < game.GoalNetTop + game.GoalNetHeight/4) AimGoalPoint[1] = game.GoalNetTop - self.Radius;// + game.GoalNetHeight/2;
			else if (self.Y < game.GoalNetTop + game.GoalNetHeight*3/4) {}
			else AimGoalPoint[1] = game.GoalNetTop + game.GoalNetHeight + self.Radius;// + game.GoalNetHeight/2;
			
			bool flag = StrikeOpps();
			if (flag){}
			else if (self.TeammateIndex == 0)
			{
				if (isGotPuck()) AimAndStrike();
				else 
				{
				HockState = HockStateType.none;
					if (world.Puck.X <= world.Width/2)gotoPuck();
					else move.Turn = self.GetAngleTo(world.Puck);
				}
			}
			else if (self.TeammateIndex == 1)
			{
				if (isGotPuck()) AimAndStrike();
				else 
				{
					HockState = HockStateType.none;
					if (world.Puck.X > world.Width/2)gotoPuck();
					else move.Turn = self.GetAngleTo(world.Puck);
				}
			}
//			double x = world.Puck.X;
//			double y = world.Puck.Y;
			
			//gotoPuck();
	
			args.Clear();
        }
        
        private bool StrikeOpps()
        {
			Hockeyist[] allguys = world.Hockeyists;
			Hockeyist[] badguys = new Hockeyist[6];
			int j = 0;
			for (int i = 0; i < allguys.Length; i++)
				if (allguys[i].Type != HockeyistType.Goalie && !allguys[i].IsTeammate) 
			{
				badguys[j] = allguys[i];
				j++;
			}
			Console.WriteLine("j = "+j.ToString());
			if (j < 2) return false;
			if (self.GetDistanceTo(badguys[0]) < 2*self.Radius &&
			    Math.Abs (self.GetAngleTo(badguys[0])) < Math.PI/4 || 
			    self.GetDistanceTo(badguys[1]) < 2*self.Radius &&
			    Math.Abs (self.GetAngleTo(badguys[1])) < Math.PI/4)
			{
			move.Action = ActionType.Strike;
			return true;
			}
			return false;
        }
        
        private double speedAbs()
        {
        	return vectorAbs(self.SpeedX, self.SpeedY);
        }
        
		private void AimAndStrike()
		{
			if (HockState == HockStateType.none) HockState = HockStateType.goingToAim;
			if (self.GetDistanceTo(AimGoalPoint[0],AimGoalPoint[1]) > 100D && HockState == HockStateType.goingToAim) 
			{
				turnToAim();
				//if (speedAbs() < 3D) 
				move.SpeedUp = 1D;//*turnToAim();
			}
			else 
			{
				HockState = HockStateType.aiming;
				//setSpeed(0);
				strikePuckWeak();
			}
		}
        
        private double turnToAim()
        {
			double angleAimAbs = Math.Abs(self.GetAngleTo(AimGoalPoint[0],AimGoalPoint[1]));
			double sign;
			if (angleAimAbs < Math.PI/4)
			{
				sign = 1;
				move.Turn = self.GetAngleTo(AimGoalPoint[0],AimGoalPoint[1]);
			}
			else if (angleAimAbs < Math.PI*3/4)
			{
				sign = 0;
				move.Turn = self.GetAngleTo(AimGoalPoint[0],AimGoalPoint[1]);
			}
			else if (self.GetAngleTo(AimGoalPoint[0],AimGoalPoint[1]) > 0)
			{
				sign = -1;
				move.Turn = Math.PI - self.GetAngleTo(AimGoalPoint[0],AimGoalPoint[1]);
			}
			else 
			{
				sign = -1;
				move.Turn = Math.PI + self.GetAngleTo(AimGoalPoint[0],AimGoalPoint[1]);
			}
			return sign;
        }
        
        
 
        
        private void setSpeed(double aimspeed)
        {
        	double currentSpeed = vectorAbs(self.SpeedX, self.SpeedY);
        	double value = aimspeed - currentSpeed;
			if (value > 1D) value = 1D;
			else if (value < -1D) value = -1D;
        	move.SpeedUp = value;
        }
        
        private double vectorAbs(double x, double y)
        {
        	return Math.Pow(x*x+y*y, 0.5);
        }
        
        private void gotoNet()
        {
			double x = world.GetMyPlayer().NetFront - self.Radius*3;
			double y = (world.GetMyPlayer().NetBottom + world.GetMyPlayer().NetTop)/2;
			
			if ((self.X - x)*(self.X - x) + (self.Y - y)*(self.Y - y) > 30D) 
				{
				move.Turn = self.GetAngleTo(x,y);
				move.SpeedUp = 0.5D;
				}
			else move.Turn = self.GetAngleTo(world.Puck);
        }
        private void def2()
        {
			if (world.Puck.X > world.Width/2) gotoPuck();
			else move.Turn = self.GetAngleTo(world.Puck);
			if (self.Id == world.Puck.OwnerHockeyistId) 
			{	
				//Tactic_RunAway1();
				strikePuckTest();
			}
        }
        
		private void gotoPuck()
		{	
			aimPuck();
			move.SpeedUp = 1.0D;
		}
		
		private bool isGotPuck()
		{
			if (self.Id == world.Puck.OwnerHockeyistId) return true;
			else return false;
		}
		
		private void aimPuck()
		{
			move.Turn = self.GetAngleTo(world.Puck);
			if (world.Puck.OwnerPlayerId != world.GetMyPlayer().Id) move.Action = ActionType.TakePuck;
		}
		private void strikePuckWeak()
		{
			double angle = self.GetAngleTo(opponent.NetFront, WeakNetSideY());
			if (Math.Abs(angle) > 0.01D) move.Turn = angle;
			else move.Action = ActionType.Strike;
		}
		
		private void strikePuckTest()
		{
			move.Action = ActionType.Swing;
			move.Turn = self.GetAngleTo(opponent.NetFront, WeakNetSideY());
			//if (self.GetAngleTo(opponent.NetFront, WeakNetSideY(opponent)) <= 0.01D) 
			move.Action = ActionType.Strike;
		}
		
		private double WeakNetSideY()
		{
			Hockeyist[] allguys = world.Hockeyists;
			Hockeyist enemyGoalie = allguys[0];
			for (int i = 0; i < allguys.Length; i++)
			{
				enemyGoalie = allguys[i];
				if (allguys[i].Type == HockeyistType.Goalie && !allguys[i].IsTeammate) break;
			}
			if (Math.Abs(opponent.NetBottom - enemyGoalie.Y) > Math.Abs(enemyGoalie.Y - opponent.NetTop))
				return (opponent.NetBottom - world.Puck.Radius);
			else return (opponent.NetTop + world.Puck.Radius);
		}
		
		private Hockeyist[] AllBadGuys()
		{
			Hockeyist[] allguys = world.Hockeyists;
			Hockeyist[] badguys = new Hockeyist[6];
			int j = 0;
			for (int i = 0; i < allguys.Length; i++)
				if (allguys[i].Type == HockeyistType.Versatile && !allguys[i].IsTeammate) 
				{
					badguys[j] = allguys[i];
					j++;
				}
			return badguys;
		}
		
		private void Init()
		{
			self = (Hockeyist)args[0];
			world = (World)args[1];
			game = (Game)args[2];
			move = (Move)args[3];
		}
		
		
		
		private double[] AveragePosition(Unit unit1, Unit unit2)
		{
			double x1 = unit1.X;
			double y1 = unit1.Y;
			double x2 = unit2.X;
			double y2 = unit2.Y;
			return new double[]{(x1+x2)/2, (y1+y2)/2};
		}
		
		private void Tactic_RunAway1()
		{
			Hockeyist[] badguys = AllBadGuys();
			double[] fromPoint = AveragePosition(badguys[0], badguys[1]);
			if (Math.Abs(self.GetAngleTo(fromPoint[0], fromPoint[1])) < Math.PI/2) move.SpeedUp = -1D;
			else move.SpeedUp = 1D;
		}
    }
}
