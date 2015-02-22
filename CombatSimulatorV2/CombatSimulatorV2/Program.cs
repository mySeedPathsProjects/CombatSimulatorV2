using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatSimulatorV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.PlayGame();
        }
    }

    class Actor
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public bool IsAlive 
        {
            get
            {
                if (HP > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public Random rng;

        public Actor(string name, int hp)
        {
            this.Name = name;
            this.HP = hp;
            this.rng = new Random();
        }

        public virtual void DoAttack(Actor actor) { }
    }

    class Enemy : Actor
    {
        public Enemy(string name, int hp) : base(name, hp) { }

        public override void DoAttack(Actor actor)
        {
            //**Attack Logic
            //**Combat text
            base.DoAttack(actor);
        }
    }

    class Player : Actor
    {
        public enum AttackType
        {
            AlkaSeltzer = 1,
            KickSand,
            AddChips,
            ChuckNorris
        };

        public Player(string name, int hp) : base(name, hp) { }

        public override void DoAttack(Actor actor)
        {
            AttackType attack = ChooseAttack();
            //**Attack Logic based on AttackType
            //**Combat text
            base.DoAttack(actor);
        }

        public AttackType ChooseAttack()
        {
            Console.Write("Choose attack type: ");
            int userAttackChoice = Convert.ToInt32(Console.ReadLine());
            //**Verify Input
            return (AttackType)userAttackChoice;
        }
    }

    class Game
    {
        public Player Oldman { get; set; }
        public Enemy Seagull { get; set; }

        public Game()
        {
            //**add a Console.ReadLine to take user input "name"??
            this.Oldman = new Player("Old Man", 20);
            this.Seagull = new Enemy("Seagulls", 30);
        }

        public void DisplayCombatInfo()
        {
            //**write to console current game info
        }

        public void PlayGame()
        {
            while (this.Oldman.IsAlive && this.Seagull.IsAlive)
            {
                DisplayCombatInfo();
                this.Oldman.DoAttack(this.Seagull);
                this.Seagull.DoAttack(this.Oldman);
            }
            if (this.Oldman.IsAlive)
            {
                Console.WriteLine("You Won!");
            }
            else
            {
                Console.WriteLine("You Lost!");
            }
        }
    }
}
