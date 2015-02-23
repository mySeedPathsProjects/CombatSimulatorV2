using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
        public string EnemySuccess { get; set; }

        public Enemy(string name, int hp) : base(name, hp) { }

        public override void DoAttack(Actor actor)
        {
            if (this.HP <= 0)
            {
                this.EnemySuccess = string.Empty;
            }
            else
            {
                int nachosTaken = 0;
                nachosTaken = this.rng.Next(1, 4);
                actor.HP -= nachosTaken;
                if (actor.HP < 0)
                {
                    actor.HP = 0;
                }
                this.EnemySuccess = "The Seagulls made off with " + nachosTaken + " of your chips!!";
            }
        }
    }

    class Player : Actor
    {
        public enum AttackType
        {
            AlkaSeltzer = 1,
            KickSand,
            AddChips,
            ChuckNorris,
            Invalid
        };

        public string PlayerSuccess { get; set; }

        public Player(string name, int hp) : base(name, hp) { }

        public override void DoAttack(Actor actor)
        {
            AttackType attack = ChooseAttack();

            switch (attack)
            {
                case AttackType.AlkaSeltzer:
                    //if the Alka-Seltzer works...
                    if (this.rng.Next(2) == 0)
                    {
                        actor.HP--;
                        //rng to see how many extra birds fly away
                        int extraBirdsFlyAway = this.rng.Next(2, 5);
                        actor.HP -= extraBirdsFlyAway;
                        //to prevent score from going below zero
                        if (actor.HP < 0)
                        {
                            actor.HP = 0;
                        }
                        //string created that prints in RoundInfo() function
                        this.PlayerSuccess = "THE ALKA-SELTZER WORKED!!  " + extraBirdsFlyAway + " other birds also flew away!!";
                        //show animation of play results
                        Console.Clear();
                        Graphics.BirdEatsAlkaSeltzer();
                        Thread.Sleep(Animations.PauseDuration);
                    }
                    else
                    {
                        this.PlayerSuccess = "Sorry, that bird is too smart for your shenanigans.";
                        //animation of play results
                        Console.Clear();
                        Graphics.BirdAvoidsAlkaSeltzer();
                        Thread.Sleep(Animations.PauseDuration * 2);
                    }
                    break;

                case AttackType.KickSand:
                    //effectiveness of sand kick
                    int sandSuccess = this.rng.Next(1, 4);
                    actor.HP -= sandSuccess;
                    if (actor.HP < 0)
                    {
                        actor.HP = 0;
                    }
                    this.PlayerSuccess = "Nice sand kick!!  " + sandSuccess + " birds flew off.";
                    //graphic of play result
                    Console.Clear();
                    Graphics.KickSand();
                    Thread.Sleep(1500);
                    break;

                case AttackType.AddChips:
                    //determine number of chips added
                    int chipsAdded = this.rng.Next(2, 5);
                    this.HP += chipsAdded;
                    this.PlayerSuccess = "You added " + chipsAdded + " chips back to your nachos.";
                    //no animation or graphics for adding chips
                    break;

                case AttackType.ChuckNorris:
                    actor.HP = 0;
                    this.PlayerSuccess = "OH YEAH!!!  You wiped them all out!!  Time to chill with some nachos!!";
                    //Chuck Norris Power animation
                    Console.Clear();
                    Graphics.ChuckNorrisFace();
                    Thread.Sleep(Animations.PauseDuration * 2);
                    break;

                case AttackType.Invalid:
                    this.PlayerSuccess = "STOP GOOFING AROUND!!";
                    break;

                default:
                    break;
            }
        }

        public AttackType ChooseAttack()
        {
            Console.Write("Enter your combat choice: ");
            string oldManCombatChoice = Console.ReadLine();
            if (this.InputValidator(oldManCombatChoice))
            {
                return (AttackType)Convert.ToInt32(oldManCombatChoice);
            }
            else
            {
                return AttackType.Invalid;
            }
        }

        /// <summary>
        /// Input needs to be a 1, 2, or 3.  Can be a 4 only after "Chuck Norris Power" is enabled
        /// </summary>
        /// <param name="userInput_">1, 2, 3, or sometimes a 4</param>
        /// <returns>true if input is valid</returns>
        private bool InputValidator(string userInput_)
        {
            //if input is more or less than 1 digit, it's invalid
            if (userInput_.Length != 1)
            {
                Console.WriteLine();
                Animations.OldTimeyTextPrinter("ENTER A VALID INPUT...", 10);
                //PlayerSuccess = "STOP GOOFING AROUND!!";
                //BirdSuccess = "YOUR NACHOS ARE IN DANGER!!";
                Thread.Sleep(Animations.PauseDuration);
                return false;
            }
            //if input is a 1, 2, or a 3 then the input is valid
            else if ("123".Contains(userInput_[0]))
            {
                return true;
            }
            //input of 4 is valid only if Chuck Norris Power is enabled, otherwise invalid
            else if (userInput_ == "4")
            {
                if (Game.ChuckNorrisPower == true)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine();
                    Animations.OldTimeyTextPrinter("ENTER A VALID INPUT...", 10);
                    //PlayerSuccess = "STOP GOOFING AROUND!!";
                    //BirdSuccess = "YOUR NACHOS ARE IN DANGER!!";
                    Thread.Sleep(Animations.PauseDuration);
                    return false;
                }
            }
            //if input is invalid (for a case i didn't think of testing for...)
            else
            {
                Console.WriteLine();
                Animations.OldTimeyTextPrinter("ENTER A VALID INPUT...", 10);
                //PlayerSuccess = "STOP GOOFING AROUND!!";
                //BirdSuccess = "YOUR NACHOS ARE IN DANGER!!";
                Thread.Sleep(Animations.PauseDuration);
                return false;
            }
        }
    }

    class Game
    {
        public Player Oldman { get; set; }
        public Enemy Seagull { get; set; }
        public int RoundCounter { get; set; }
        public static bool ChuckNorrisPower { get; set; }
        private Random rng;

        public Game()
        {
            //**add a Console.ReadLine to take user input "name"??
            this.Oldman = new Player("Old Man", 20);
            this.Seagull = new Enemy("Seagulls", 30);

            Console.SetWindowSize(116, 50);
            this.rng = new Random();

            //Animations.IntroAnimation();
            //Animations.TitleSequence();
            //Animations.Instructions();
        }

        public void PlayGame()
        {
            this.RoundCounter = 0;
            Game.ChuckNorrisPower = false;
            while (this.Oldman.IsAlive && this.Seagull.IsAlive)
            {
                Game.ChuckNorrisPower = AllowCNPower();
                DisplayCombatInfo();
                this.Oldman.DoAttack(this.Seagull);
                this.Seagull.DoAttack(this.Oldman);
                this.RoundCounter++;
            }
            if (this.Oldman.IsAlive)
            {
                OldManWins();
            }
            else
            {
                SeagullsWin();
            }
        }

        private bool AllowCNPower()
        {
            if (Game.ChuckNorrisPower == false)
            {
                if (RoundCounter > 4)
                {
                    if (3 >= this.rng.Next(1, 11))
                    {
                        return true;
                    } return false;
                } return false;
            } return true;
        }

        private void DisplayCombatInfo()
        {
            //**write to console current game info
            this.RoundInfo();
            this.BasicInstructions();
        }

        /// <summary>
        /// Displays information from previous round of game play
        /// </summary>
        private void RoundInfo()
        {
            Console.Clear();
            Graphics.SeagullShowdownText_2();

            //after first round 
            if (this.RoundCounter > 0)
            {
                //Print results of Player and Bird success results from last round of play
                Animations.OldTimeyTextPrinter(this.Oldman.PlayerSuccess, 10);
                Thread.Sleep(Animations.PauseDuration / 2);
                Console.WriteLine();
                Console.WriteLine();
                Animations.OldTimeyTextPrinter(this.Seagull.EnemySuccess, 10);
                Thread.Sleep(Animations.PauseDuration);
            }
            //when game first starts, before there are any play results yet
            else
            {
                Thread.Sleep(Animations.PauseDuration / 2);
                Animations.OldTimeyTextPrinter("NEVER GIVE UP!!", 10);
                Thread.Sleep(Animations.PauseDuration / 2);
                Console.WriteLine();
                Console.WriteLine();
                Animations.OldTimeyTextPrinter("THESE BIRDS WILL STOP AT NOTHING!!", 10);
                Thread.Sleep(Animations.PauseDuration);
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            //print number of birds and nachos remaining and include graphical representation of score
            Console.Write(("Number of remaining BIRDS:  " + this.Seagull.HP).PadRight(33));
            for (int i = 0; i < this.Seagull.HP; i++)
            {
                Console.Write("*");
            }
            Console.WriteLine();
            //Console.WriteLine();
            Console.Write(("Number of remaining NACHOS: " + this.Oldman.HP).PadRight(33));
            for (int i = 0; i < this.Oldman.HP; i++)
            {
                Console.Write("*");
            }
            Console.WriteLine();
            Console.WriteLine();

        }

        /// <summary>
        /// Basic instructions of game printed every round
        /// </summary>
        private void BasicInstructions()
        {
            //print basic instructions
            Console.WriteLine("Enter 1 to use an Alka-Seltzer tablet...");
            Console.WriteLine("Enter 2 to kick sand at the damn birds...");
            Console.WriteLine("Enter 3 to replenish your chips if running low...");
            Console.WriteLine();
            //if Chuck Norris Power is turned on print following text to inform player and tell them how to use it.
            if (Game.ChuckNorrisPower == true)
            {
                Graphics.ChuckNorrisPowerText();
                Console.WriteLine();
                Console.WriteLine("***HOLY SMOKES!!***  The Gods have bestowed upon you CHUCK NORRIS POWER!!");
                Console.WriteLine("Enter 4 to throw a tornado of sand at the birds and wipe them out!!!  DO IT!!!");
                Console.WriteLine();
                Console.WriteLine();
            }
            //Console.Write("Enter your combat choice: ");
        }

        private void OldManWins()
        {
            //show final round info
            Console.Clear();
            Graphics.SeagullShowdownText_2();
            RoundInfo();
            Thread.Sleep(Animations.PauseDuration * 3);
            Console.Clear();
            Console.CursorVisible = false;
            //animation if you won
            for (int i = 0; i < 3; i++)
            {
                Graphics.YouWon();
                Thread.Sleep(Animations.PauseDuration / 6);
                Console.Clear();
                Thread.Sleep(Animations.PauseDuration / 6);
            }
            Graphics.YouWon();
            Console.WriteLine();
            Animations.OldTimeyTextPrinter("         Damn birds are no match for your awesome skills!!", 20);
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(Animations.PauseDuration);
            //ask user to play again
            this.PlayAgain();
        }

        private void SeagullsWin()
        {
            //show final round info
            Console.Clear();
            Graphics.SeagullShowdownText_2();
            RoundInfo();
            Thread.Sleep(Animations.PauseDuration * 3);
            Console.Clear();
            Console.CursorVisible = false;
            //animation if you lost
            for (int i = 0; i < 3; i++)
            {
                Graphics.YouLost();
                Thread.Sleep(Animations.PauseDuration / 6);
                Console.Clear();
                Thread.Sleep(Animations.PauseDuration / 6);
            }
            Graphics.YouLost();
            Console.WriteLine();
            Animations.OldTimeyTextPrinter("         You can't win against some pesky birds???", 20);
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(Animations.PauseDuration);
            //ask user if they want to play again
            this.PlayAgain();
        }

        /// <summary>
        /// ask user to play again, run game again if yes, exit game if no
        /// </summary>
        private void PlayAgain()
        {
            Console.WriteLine();
            Console.Write("Do you want to play again, Y or N: ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                this.Oldman.HP = 20;
                this.Seagull.HP = 30;
                this.PlayGame();
            }
        }

    }


    public static class Animations
    {
        public static int PauseDuration = 1000;

        /// <summary>
        /// Prints text to screen one char at a time
        /// </summary>
        /// <param name="inputText">text you want to print</param>
        /// <param name="pause">time between each digit printing</param>
        public static void OldTimeyTextPrinter(string inputText, int pause)
        {
            //loop through each character
            for (int i = 0; i < inputText.Length; i++)
            {
                char letter = inputText[i];
                Console.Write(letter);
                Thread.Sleep(pause);
            }
        }

        /// <summary>
        /// animation sequence to start off game
        /// </summary>
        public static void IntroAnimation()
        {
            Console.CursorVisible = false;
            Graphics.IntroAniGraphics(0);
            Thread.Sleep(PauseDuration);
            Console.SetCursorPosition(15, 18);
            OldTimeyTextPrinter("Man....I love being retired.", 30);
            Thread.Sleep(PauseDuration * 2);
            Console.SetCursorPosition(15, 18);
            OldTimeyTextPrinter("                              ", 8);
            Console.SetCursorPosition(15, 18);
            OldTimeyTextPrinter("Nothing like enjoying some NACHOS at the beach.", 30);
            Thread.Sleep(PauseDuration * 2);
            Console.SetCursorPosition(15, 18);
            OldTimeyTextPrinter("                                                ", 8);
            Thread.Sleep(PauseDuration * 2);

            Console.Clear();
            Graphics.IntroAniGraphics(1);
            Thread.Sleep(PauseDuration);
            Console.SetCursorPosition(15, 18);
            OldTimeyTextPrinter("OH CRAP!!  NOT AGAIN....!", 30);
            Thread.Sleep(PauseDuration);

            Console.Clear();
            Graphics.IntroAniGraphics(2);
            Thread.Sleep(PauseDuration);
            Console.Clear();
            Graphics.IntroAniGraphics(3);
            Thread.Sleep(PauseDuration / 4);
            Console.Clear();
            Graphics.IntroAniGraphics(4);
            Thread.Sleep(PauseDuration / 2);
            Console.SetCursorPosition(15, 32);
            OldTimeyTextPrinter("GET AWAY FROM MY NACHOS YOU DAMN BIRDS!!!", 30);
            Thread.Sleep(PauseDuration * 3);
        }

        /// <summary>
        /// more animation...title of game and picture of players
        /// </summary>
        public static void TitleSequence()
        {
            Console.Clear();
            Graphics.SeagullShowdownText_1();
            Thread.Sleep(PauseDuration / 2);
            Console.Clear();
            Graphics.SeagullShowdownText_2();
            Thread.Sleep(PauseDuration / 2);
            Graphics.ManandSeagull_2();
            Thread.Sleep(PauseDuration);

            //make title of game "blink" 3 times
            for (int i = 0; i < 3; i++)
            {
                Console.Clear();
                Graphics.SeagullShowdownText_3();
                Graphics.ManandSeagull_2();
                Thread.Sleep(PauseDuration / 4);
                Console.Clear();
                Graphics.SeagullShowdownText_2();
                Graphics.ManandSeagull_2();
                Thread.Sleep(PauseDuration / 4);
            }

            Thread.Sleep(PauseDuration * 2);

        }

        /// <summary>
        /// Prints instructions on how to play the game.
        /// </summary>
        public static void Instructions()
        {
            Console.CursorVisible = false;
            Console.Clear();

            Graphics.SeagullShowdownText_2();
            Thread.Sleep(PauseDuration / 2);
            Console.WriteLine();
            OldTimeyTextPrinter("Man, you've been through this crap before.  Just trying to enjoy some nachos at the beach \nand a swarm of Seagulls come and eat all your food, ruining the day.", 20);
            Thread.Sleep(PauseDuration / 2);
            Console.WriteLine();
            Console.WriteLine();
            OldTimeyTextPrinter("This time you're fighting back!!  You've brought a bunch of Alka-Seltzer with you...\nknowing that Seagulls blow up if they eat one.", 20);
            Thread.Sleep(PauseDuration / 2);
            Console.WriteLine();
            Console.WriteLine();
            OldTimeyTextPrinter("You've also been honing your skills at kicking sand.  You're totally ready for this!!", 20);
            Thread.Sleep(PauseDuration);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("       COMBAT CHOICES:");
            Thread.Sleep(PauseDuration);
            OldTimeyTextPrinter("**Alka-Seltzer kills a bird and causes others to fly away...IF the bird eats it.", 10);
            Console.WriteLine();
            Thread.Sleep(PauseDuration / 4);
            OldTimeyTextPrinter("**Kicking sand always works, but only makes a few birds fly away at most.", 10);
            Console.WriteLine();
            Thread.Sleep(PauseDuration / 4);
            OldTimeyTextPrinter("**Add more chips to the nachos if you almost run out (2-4).", 10);
            Console.WriteLine();
            Thread.Sleep(PauseDuration);
            Console.WriteLine();
            OldTimeyTextPrinter("WORK FAST!!!  The Seagulls are attacking your plate the whole time, grabbing up to several chips at a time!!", 20);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(PauseDuration);

            //instruction screen will remain until user presses a key to start the game
            Console.WriteLine("Press any key to continue: ");
            Console.ReadKey();
        }

    }

    public static class Graphics
    {
        public static void IntroAniGraphics(int picNumber_)
        {
            switch (picNumber_)
            {
                case 0:
                    Console.WriteLine(@"
                 

                                                                              .     :     .
                                                                            .  :    |    :  .
                                                                             .  |   |   |  ,
                                                                              \  |     |  /
                                                                          .     ,-''''`-.     .
                                                                            '- /         \ -'
                                                                              |           |
                                                                        - --- |           | --- -
                                                                              |           |
                                                                            _- \         / -_
                                                                          .     `-.___,-'     .
                                                                              /  |     |  \
                                                                            .'  |   |   |  `.
                                                                               :    |    :
                                                                              .     :     .
                                                                                    .
 

              

    ---....___________________________________  ___ _______ __ _ ___ ___ __ _ _ _____ _ _ ___ __  _ ___ _____
                                              ---...__ =-= = -_= -=_= _-=_-_ -=- =-_=_-_ -=-== _-=_-_ -=- =-
                                                     ---...___ =-= = -_= -=_= _-=_-_ -=- =-_=_-_ -=-== 
                          <()>                                 ```--.._= -_= -_= _-=- -_= _=-_= _-=- -_
                           #\                                        ``--._=-_ =-=_-= _-= _=-_ =-=_-
                           ##\/\                                           ``-._=_-=_- =_-=_= _-=- --
                           #####\_                                           `-._-=_-_=-=-_ =-=_=-
                                                                                 `-._=-_=-=_-= _-= _-=_-_ -=- =-
                                                                                   `-._= _-=- -_-=_-_ -=- =-
                                                                                      `-._=-_=-=_-= _-= 

");
                    break;

                case 1:
                    Console.WriteLine(@"
                 

                                                                              .     :     .
                                                                            .  :    |    :  .
                       _,___                                                 .  |   |   |  ,
             __.=""=._/' /---\                                                 \  |     |  /
            `'=.__,    (                                                  .     ,-''''`-.     .
            ,'=='   ;  `=,                                                  '- /         \ -'
            `^`^`^'`',    ;                                                   |           |
                      '; (                                              - --- |           | --- -
                        ``                                                    |           |
                                                                            _- \         / -_
                                                                          .     `-.___,-'     .
                                                                              /  |     |  \
                                                                            .'  |   |   |  `.
                                                                               :    |    :
                                                                              .     :     .
                                                                                    .
 

              

    ---....___________________________________  ___ _______ __ _ ___ ___ __ _ _ _____ _ _ ___ __  _ ___ _____
                                              ---...__ =-= = -_= -=_= _-=_-_ -=- =-_=_-_ -=-== _-=_-_ -=- =-
                                                     ---...___ =-= = -_= -=_= _-=_-_ -=- =-_=_-_ -=-== 
                          <()>                                 ```--.._= -_= -_= _-=- -_= _=-_= _-=- -_
                           #\                                        ``--._=-_ =-=_-= _-= _=-_ =-=_-
                           ##\/\                                           ``-._=_-=_- =_-=_= _-=- --
                           #####\_                                           `-._-=_-_=-=-_ =-=_=-
                                                                                 `-._=-_=-=_-= _-= _-=_-_ -=- =-
                                                                                   `-._= _-=- -_-=_-_ -=- =-
                                                                                      `-._=-_=-=_-= _-= 

");
                    break;

                case 2:
                    Console.WriteLine(@"
                 

                                                                              .     :     .
                                                                            .  :    |    :  .
                       _,___                                                 .  |   |   |  ,
             __.=""=._/' /---\                                                 \  |     |  /
            `'=.__,    (                                                  .     ,-''''`-.     .
            ,'=='   ;  `=,                                                  '- /         \ -'
            `^`^`^'`',    ;                                                   |           |
                      '; (                        ___   ________        - --- |           | --- -
                        ``                       /---<9;/  ,__-==`            |           |
                                                ./~~( `)/`                  _- \         / -_
                                             ,-'_/// \  }                 .     `-.___,-'     .
                                              ~       XX\\                    /  |     |  \
                                                          \                 .'  |   |   |  `.
                                                                               :    |    :
                                                                              .     :     .
                                                                                    .
 

              

    ---....___________________________________  ___ _______ __ _ ___ ___ __ _ _ _____ _ _ ___ __  _ ___ _____
                                              ---...__ =-= = -_= -=_= _-=_-_ -=- =-_=_-_ -=-== _-=_-_ -=- =-
                         \    /                      ---...___ =-= = -_= -=_= _-=_-_ -=- =-_=_-_ -=-== 
                          \()/                                 ```--.._= -_= -_= _-=- -_= _=-_= _-=- -_
                           #\                                        ``--._=-_ =-=_-= _-= _=-_ =-=_-
                           ##\/\                                           ``-._=_-=_- =_-=_= _-=- --
                           #####\_                                           `-._-=_-_=-=-_ =-=_=-
                                                                                 `-._=-_=-=_-= _-= _-=_-_ -=- =-
                                                                                   `-._= _-=- -_-=_-_ -=- =-
                                                                                      `-._=-_=-=_-= _-= 

");
                    break;

                case 3:
                    Console.WriteLine(@"
 
                
                                                                              .     :     .
                                                                            .  :    |    :  .
                       _,___                                                 .  |   |   |  ,
             __.=""=._/' /---\                                                 \  |     |  /
            `'=.__,    (                                                  .     ,-''''`-.     .
            ,'=='   ;  `=,                                                  '- /         \ -'
            `^`^`^'`',    ;                                                   |           |
                      '; (                        ___   ________        - --- |           | --- -
                        ``                       /---<9;/  ,__-==`            |           |
                                                ./~~( `)/`                  _- \         / -_
                                             ,-'_/// \  }                 .     `-.___,-'     .
                                              ~       XX\\                    /  |     |  \
      .`.   _ ____                                        \                 .'  |   |   |  `.
    __;_ \ /,//---\                                                            :    |    :
    --, `._) (                                                                .     :     .
     '//,,,  |                                                                      .
          )_/                
         /_|                 
                             
                                
    ---....___________________________________  ___ _______ __ _ ___ ___ __ _ _ _____ _ _ ___ __  _ ___ _____
                                              ---...__ =-= = -_= -=_= _-=_-_ -=- =-_=_-_ -=-== _-=_-_ -=- =-
                         \    /                      ---...___ =-= = -_= -=_= _-=_-_ -=- =-_=_-_ -=-== 
                          \()/                                 ```--.._= -_= -_= _-=- -_= _=-_= _-=- -_
                           #\                                        ``--._=-_ =-=_-= _-= _=-_ =-=_-
                           ##\/\                                           ``-._=_-=_- =_-=_= _-=- --
                           #####\_                                           `-._-=_-_=-=-_ =-=_=-
                                                                                 `-._=-_=-=_-= _-= _-=_-_ -=- =-
                                                                                   `-._= _-=- -_-=_-_ -=- =-
                                                                                      `-._=-_=-=_-= _-= 

");
                    break;

                case 4:
                    Console.WriteLine(@"
 
                
                                                                              .     :     .
                                                                            .  :    |    :  .
                       _,___                                                 .  |   |   |  ,
             __.=""=._/' /---\                                                 \  |     |  /
            `'=.__,    (                                                  .     ,-''''`-.     .
            ,'=='   ;  `=,                                                  '- /  __ __  \ -'
            `^`^`^'`',    ;                                                   |  | .I. |  |
                      '; (                        ___   ________        - --- |   --^--   | --- -
                        ``                       /---<9;/  ,__-==`            |    ___    |
                                                ./~~( `)/`                  _- \  (___)  / -_
                                             ,-'_/// \  }                 .     `-.___,-'     .
                                              ~       XX\\                    /  |     |  \
      .`.   _ ____                                        \                 .'  |   |   |  `.
    __;_ \ /,//---\                                                            :    |    :
    --, `._) (                                                                .     :     .
     '//,,,  |                              ,                                       .
          )_/                 ,_     ,     .'<_
         /_|                 _> `'-,'(__.-' __<
                             >_.--(.. )  =;`
                                  `V-'`'\/``
    ---....___________________________________  ___ _______ __ _ ___ ___ __ _ _ _____ _ _ ___ __  _ ___ _____
                                              ---...__ =-= = -_= -=_= _-=_-_ -=- =-_=_-_ -=-== _-=_-_ -=- =-
                         \    /                      ---...___ =-= = -_= -=_= _-=_-_ -=- =-_=_-_ -=-== 
                          \()/                                 ```--.._= -_= -_= _-=- -_= _=-_= _-=- -_
                           #\                                        ``--._=-_ =-=_-= _-= _=-_ =-=_-
                           ##\/\                                           ``-._=_-=_- =_-=_= _-=- --
                           #####\_                                           `-._-=_-_=-=-_ =-=_=-
                                                                                 `-._=-_=-=_-= _-= _-=_-_ -=- =-
                                                                                   `-._= _-=- -_-=_-_ -=- =-
                                                                                      `-._=-_=-=_-= _-= 

");
                    break;

                default:
                    break;
            }
        }

        public static void SeagullShowdownText_1()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"
  _____   ___   ____   ____  __ __  _      _         
 / ___/  /  _] /    T /    T|  T  T| T    | T        
(   \_  /  [_ Y  o  |Y   __j|  |  || |    | |       
 \__  TY    _]|     ||  T  ||  |  || l___ | l___      
 /  \ ||   [_ |  _  ||  l_ ||  :  ||     T|     T    
 \    ||     T|  |  ||     |l     ||     ||     |    
  \___jl_____jl__j__jl___,_j \__,_jl_____jl_____j      
                                                                                                                   
");
            Console.WriteLine();
        }

        public static void SeagullShowdownText_2()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"
  _____   ___   ____   ____  __ __  _      _           _____ __ __   ___   __    __  ___     ___   __    __  ____  
 / ___/  /  _] /    T /    T|  T  T| T    | T         / ___/|  T  T /   \ |  T__T  T|   \   /   \ |  T__T  T|    \ 
(   \_  /  [_ Y  o  |Y   __j|  |  || |    | |        (   \_ |  l  |Y     Y|  |  |  ||    \ Y     Y|  |  |  ||  _  Y
 \__  TY    _]|     ||  T  ||  |  || l___ | l___      \__  T|  _  ||  O  ||  |  |  ||  D  Y|  O  ||  |  |  ||  |  |
 /  \ ||   [_ |  _  ||  l_ ||  :  ||     T|     T     /  \ ||  |  ||     |l  `  '  !|     ||     |l  `  '  !|  |  |
 \    ||     T|  |  ||     |l     ||     ||     |     \    ||  |  |l     ! \      / |     |l     ! \      / |  |  |
  \___jl_____jl__j__jl___,_j \__,_jl_____jl_____j      \___jl__j__j \___/   \_/\_/  l_____j \___/   \_/\_/  l__j__j
                                                                                                                   
");
            Console.WriteLine();
        }

        public static void SeagullShowdownText_3()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(@"
  _____   ___   ____   ____  __ __  _      _           _____ __ __   ___   __    __  ___     ___   __    __  ____  
 / ___/  /  _] /    T /    T|  T  T| T    | T         / ___/|  T  T /   \ |  T__T  T|   \   /   \ |  T__T  T|    \ 
(   \_  /  [_ Y  o  |Y   __j|  |  || |    | |        (   \_ |  l  |Y     Y|  |  |  ||    \ Y     Y|  |  |  ||  _  Y
 \__  TY    _]|     ||  T  ||  |  || l___ | l___      \__  T|  _  ||  O  ||  |  |  ||  D  Y|  O  ||  |  |  ||  |  |
 /  \ ||   [_ |  _  ||  l_ ||  :  ||     T|     T     /  \ ||  |  ||     |l  `  '  !|     ||     |l  `  '  !|  |  |
 \    ||     T|  |  ||     |l     ||     ||     |     \    ||  |  |l     ! \      / |     |l     ! \      / |  |  |
  \___jl_____jl__j__jl___,_j \__,_jl_____jl_____j      \___jl__j__j \___/   \_/\_/  l_____j \___/   \_/\_/  l__j__j
                                                                                                                   
");
            Console.WriteLine();
        }

        public static void ManandSeagull_2()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"
                                                   
                .-'--.                                                                  _______        
              .'      '.                                                            _.-'       ''...._
             /     _    `-.                                                       .'        .--.    '.`
            /      .\-     \,  ,                                                 : .--.    :    :     '-.
           ;       .-|-'    \####,                                              : :    :   :    :       :`
           |,       .-|-'    ;####                                              : :  @ :___:  @ : __     '`.
          ,##         `     ,|###'                                       _____..:---''''   `----''  `.   .''
        #,####, '#,        ,#|^;#                                     -''                      ___j  :   :
        `######  `#####,|##' |`)|                                    /                   __..''      :    `.
         `#####    ```o\`\o_.| ;\                                   /---------_______--''        __..'   /``
          (-`\#,    .-'` |`  : `;                                   \ _______________________--''       /
          `\ ;\#,         \   \-'                                                    --''               \
            )( \#    C,_   \   ;                                                     :                :`.:
            (_,  \  /   `'./   |                                                      :              /
              \  / | .-`'--'`. |                                                       \            /
               | ( \   ,  /_,  |                                                        \          .'
               \    `   ``     /                                                         :         :
                '-.__     // .'                                                           :        \
                     `'`.__.'                                                             :         \

");

        }

        public static void ChuckNorrisPowerText()
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(@"
  _______            __     _  __             _       ___                    
 / ___/ /  __ ______/ /__  / |/ /__  ________(_)__   / _ \___ _    _____ ____
/ /__/ _ \/ // / __/  '_/ /    / _ \/ __/ __/ (_-<  / ___/ _ \ |/|/ / -_) __/
\___/_//_/\_,_/\__/_/\_\ /_/|_/\___/_/ /_/ /_/___/ /_/   \___/__,__/\__/_/   
                                                                             ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void ChuckNorrisFace()
        {
            Console.WriteLine(@"                                                                                
                                ..MMMMMMMMMMM. ..                               
                              .$MMMMMMMMMMMMMMMMM..                             
                            ..MMMMMMMMMMMMMMMMMMMM .                            
                           .MMMMMMMMMMMMMMMMMMMMMMM .                           
                          .MMMMMMMMMMMMMMMMMMMMMMMMM..                          
                          MMMMMMMMMMMMMMMMMMMMMMMMMMM.                          
                        .MMMMMMMMMMMMMMMMMMMMMMMMMMMM:                          
                        OMMMMMMMMMMMMMMMMMMMMMMMMMMMMM               
                        MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMI..            
                        8MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM:          
         $MMMMMMMMM$,.   MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM.       
      MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMD    
    OMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM    
    MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM   
    MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM   
    MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM 
     MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM  
     MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM..MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
      MMMMMMMMMMMMMMMMMMM    MMMMMM.  7MMMMMD       MMMMMMMMMMMMMMMMMMMMMMMMMM 
      ?MMMMMMMMMMMMMMMMMO  MMMMMMMM      MMMMM8  .   MMMMMMMMMMMMMMMMMMMMMMMMM
       NMMMMMMMMMMMMMMMMN  ....MMM .      ... ....    MMMMMMMMMMMMMMMMMMMMMMMM 
        NMMMMMMMMMMMMMMMM.      M, .                  ,MMMMMMMMMMMMMMMMMMMMMM+ 
         8MMMMMMMMMMMMMMM       M.                    .MMMMMMMMMMMMMMMMMMMMMM  
           MMMMMMMMMMMMMM ..                     .    ZMMMMMMMMMMMMMMMMMMMMM    
            MMMMMMMMMMMMM       ...    .          . ..MMMMMMMMMMMMMMMMMMMMM  
              MMMMMMMMMMM       MMM~.MMM.          ..MMMMM MMMMMMMMMMMMMMM    
               ~MMMMMMMMM        MMMM               MMM.O. MMMMMMMMMMMMMM   
                  MMMMMMM    .MMMMMMMMMMMD          IM  .  MMMMMMMMMMMM     
                     MMMM. .MMMMMMMMMMMMMMMM        IM.  .MMMMMMMMMMM     
                        M .MMMMMMMMMMMMMMMMMM    ..DMM . OMMMMMMMMM         
                        M.MMMN           8MMMM, ..~MMM. +MMMMMMMM              
                        MMMM               ~MMM..IMMMM..MMMMMMMMMI              
                        MMMMM    MMM        ZMMMMMMMMM8.MMMMMMMMMM            
                        MMMMMMMMMMMMMMMMMMMMMMMMMMMM8 .8MMMMMMMM              
                        7MMMMMMMMMMMMMMMMMMMMMMMMMMM . ,MMMMMMMMM               
                         OMMMMMMMMMMMMMMMMMMMMMMMMM.    MMMMMMMMM              
                          NMMMMMMMMMMMMMMMMMMMMMMZ      ? MMMMMMM              
                           $MMMMMMMMMMMMMMMMMMMM,..     .MMMMMM8MM             
                             MMMMMMMMMMMMMMMMM. .        ?MMMMM             
                               MMMM=7MMMMMM, ...      ..MMMMMM            
                                   .. . . . .         MMMMMMMMO                 
                                                   .MMMMMMMMMMM                 
                              M                 ..DMMMMMMMMMMMMM              
                             MMM      .  .N    .~MMMMMMMMMMMMMMM               
                           .MMMMM     .MM.    .MMMMMMMMMMMMMMMMMM             
                          7MMMMMMN    ....   MMMMMMMMMMMMMMMMMMMM              
                         NMMMMMMMM.        MMMMMMMMMMMMMMMMMMMMM               
                        MMMMMMMMMMM      MMMMMMMMMMMMMMMMMMMMM          
");
        }

        public static void BirdEatsAlkaSeltzer()
        {
            Console.WriteLine(@"


                                             _______
                                         _.-'       ''...._
                                       .'        .--.    '.`
                                      : .--.    :    :     '-.
                                     : :    :   :    :       :`
                                     : :  @ :___:  @ : __     '`.
                              _____..:---''''   `----''  `.   .''
                      	   -''                      ___j  :   :
                          /                   __..''      :    `.
                         /---------_______--''        __..'   /``
                         \ _______________________--''       /
                                          --''               \
                                          :                :`.:
                                           :              /
                                            \            /
                                             \          .'
                                              :         :
                                               :        \
                                               :         \

");
            Thread.Sleep(Animations.PauseDuration);
            Console.Clear();

            Console.WriteLine(@"


                                             _______
                                         _.-'       ''...._
                                       .'        .--.    '.`
                                      : .--.    :    :     '-.
                                     : :    :   :    :       :`
                                     : :  @ :___:  @ : __     '`.
                              _____..:---''''   `----''  `.   .''
                      	   -''                            :   :
                          /                               :    `.
                         /--------@@-------------\    __..'   /``
                          ________@@_____________/--''       /
                         \ _______________________:          \
                                          :                :`.:
                                           :              /
                                            \            /
                                             \          .'
                                              :         :
                                               :        \
                                               :         \

");
            Thread.Sleep(Animations.PauseDuration / 2);
            Console.Clear();

            Console.WriteLine(@"


                                             _______
                                         _.-'       ''...._
                                       .'        .--.    '.`
                                      : .--.    :    :     '-.
                                     : :    :   :    :       :`
                                     : :  @ :___:  @ : __     '`.
                              _____..:---''''   `----''  `.   .''
                      	   -''                      ___j  :   :
                          /                   __..''      :    `.
                         /---------_______--''        __..'   /``
                         \ _______________________--''       /
                                          --''               \
                                          :                :`.:
                                           :              /
                                            \            /
                                             \          .'
                                              :         :
                                               :        \
                                               :         \

");
            Thread.Sleep(Animations.PauseDuration / 2);
            Console.Clear();

            Console.WriteLine(@"


                                             _______
                                         _.-'    .----...._
                                       .'----.  :      : '.`
                                      ::      : :      :   '-.
                                     : :   X  : :   X  :     :`
                                     : :      :_:      :      '`.
                              _____..:---''''   `----'        .''
                      	   -''                            \   :
                          /                               :    `.
                         /-------\                    __..'   /``
                         \ _______________________--''       /
                                          --''               \
                                          :                :`.:
                                           :              /
                                            \            /
                                             \          .'
                                              :         :
                                               :        \
                                               :         \

");
            Thread.Sleep(Animations.PauseDuration);
            Console.Clear();

            Console.WriteLine(@"






                        ██████╗  ██████╗  ██████╗ ███╗   ███╗██╗██╗
                        ██╔══██╗██╔═══██╗██╔═══██╗████╗ ████║██║██║
                        ██████╔╝██║   ██║██║   ██║██╔████╔██║██║██║
                        ██╔══██╗██║   ██║██║   ██║██║╚██╔╝██║╚═╝╚═╝
                        ██████╔╝╚██████╔╝╚██████╔╝██║ ╚═╝ ██║██╗██╗
                        ╚═════╝  ╚═════╝  ╚═════╝ ╚═╝     ╚═╝╚═╝╚═╝
                                           

");



        }

        public static void BirdAvoidsAlkaSeltzer()
        {
            Console.WriteLine(@"


                                             _______
                                         _.-'       ''...._
                                       .'        .--.    '.`
                                      : .--.    :    :     '-.
                                     : :    :   :    :       :`
                                     : :  @ :___:  @ : __     '`.
                              _____..:---''''   `----''  `.   .''
                      	   -''                      ___j  :   :
                          /                   __..''      :    `.
                         /---------_______--''        __..'   /``
                         \ _______________________--''       /
                                          --''               \
                                          :                :`.:
                                           :              /
                                            \            /
                                             \          .'
                                              :         :
                                               :        \
                                               :         \

");
            Thread.Sleep(Animations.PauseDuration);
            Console.Clear();
            Console.WriteLine(@"


                                             _______
                                         _.-'       ''...._
                                       .' \        /     '.`
                                      : .--\      /--.     '-.
                                     : :____\   /____:       :`
                                     : :--@-:___:--@-:        '`.
                              _____..:---''''   `----''__     .''
                      	   -''                           \    :
                          /        _________              :    `.
                         /---------         -----___      :   /``
                         \ ____------------_____    \     /  /
                                            ''        ---    \
                                            :              :`.:
                                            :             /
                                            \            /
                                             \          .'
                                              :         :
                                               :        \
                                               :         \

");

        }

        public static void KickSand()
        {
            Console.WriteLine(@"
                                                                *   *
                                                      *      *          *     *
                                                      
                       \|//                      *        *         *       *
                     -/_ /            ,-.   *         *
                       _\\_           |  \    *     *           *
                       \_  \          x  |   *   *      *                 *
                 ,///   >   )         \_  \     *     *             *
                / + +\ /   /         _/ )_/         *           *               *
                |     )  \/        _/ \/        *     *     * *
                /\__D/    \      _/    )                  *
                 /  _   o  \   _/,   _/        *    *           *   *
                /   /       ,_/   __/            *      *
               /   / \    o//    _/       * 
              /__o/   \___|    _/               *
              _//       \__ __/\            *               *
              \  \>       \     \            
              // |         \__   \            
                            /    /
                            \___(
                            /_/
                           / O \
                           '-   \__
                             \_____)  

");
        }

        public static void YouWon()
        {
            Console.WriteLine(@"
                                                       ______ __          
                                                     {-_-_= '. `'.          
                                                      {=_=_-  \   \     
                       \|//                      *     {_-_   |   /   
                     -/_ /            ,-.   *           '-.   |  /    .===,
                       _\\_           |  \    *      .--.__\  |_(_,==`  ( o)'-...
                       \_  \          x  |   *      `---.=_ `     ;      `/ -----\
                 ,///   >   )         \_  \             `,-_       ;    .'
                / + +\ /   /         _/ )_/               {=_       ;=~`    
                |     )  \/        _/ \/                   `//__,-=~`
                /\__D/    \      _/    )                   <<__ \\__
                 /  _   o  \   _/,   _/                    /`)))/`)))
                /   /       ,_/   __/       
               /   / \    o//    _/        
              /__o/   \___|    _/          
              _//       \__ __/\            
              \  \>       \     \            
              // |         \__   \            
                            /    /
                            \___(
                            /_/
                           / O \
                           '-   \__
                             \_____)  

");
        }

        public static void YouLost()
        {
            Console.WriteLine(@"
                                               _
                                              ~\\_
                                                \\\\
                                               `\\\\\
                         |                       |\\\\\
          \_            /;                        \\\\\|__.--~~\
          `\~--.._     //'                     _--~            /
           `//////\  \\/;'                ___/~ //////  _-~~~~'
             ~/////\~\`---\             /____'-//////-//
                 `~'  |                      //////(((-)
                 ;'_\'\                    /////
                /~/ '' ''               _///'                
               `\/'                    ~


                       _O/                
                         \        
                         /\_         
                         \      
        
");
        }

    }
}
