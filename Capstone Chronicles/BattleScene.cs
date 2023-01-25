using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Capstone_Chronicles.GUI;

namespace Capstone_Chronicles
{
    internal class BattleScene : Scene
    {
        static Hero curHero = HeroManager.Party[0];
        static List<Enemy> enemies;
        static int i2 = 0;

        //GUI
        static Menu actionMenu = new Menu(
            new Label(() =>
                {
                    actionMenu.Prompt.Text = "What will {0} do?\nHP: {1}/{2} | SP: {3}/{4}";
                    actionMenu.Prompt.SetText(actionMenu.Prompt.Text, 
                        curHero.Name, curHero.Hp, curHero.MaxHp, curHero.Sp, curHero.MaxSp);
                }),
            new Button[]
            {
                new Button("Attack", menu =>
                {
                    enemies[i2].ModifyHP(-curHero.Attack);
                })
            });
        //

        public BattleScene(params Enemy[] inEnemies) 
            : base(Array.Empty<GUIComponent>())
        {
            enemies = inEnemies.ToList();

            AddGUIElement(actionMenu, false);

            //Must come last
            RunBattle();
        }

        private void RunBattle()
        {
            while (RunRound());
        }

        /// <summary>
        /// Runs battle logic for one round
        /// </summary>
        /// <returns> True if the battle is still going; False if the battle is over </returns>
        private bool RunRound()
        {
            var heroes = new List<Hero>();
            for (int i = 0; i < HeroManager.Party.Count; i++)
            {
                if (HeroManager.Party[i].Hp > 0)
                    heroes.Add(HeroManager.Party[i]);
            }

            //actionMenu.Prompt.UpdateData(curHero);
            actionMenu.Enabled = true;
            Refresh();
            
            heroes[0].ModifyHP(-enemies[0].Attack);
            enemies.RemoveAll(x => x.Hp <= 0);
            //curHero = HeroManager.Party[1];

            if (enemies.Count == 0)
                return false;
            return true;
        }
    }
}
