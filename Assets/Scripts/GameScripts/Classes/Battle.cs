using UnityEngine;
using System.Collections;

/*A battle holds info about the attackers and defenders while they wait for the battle to be resolved*/
public class Battle {
    
    Army attacker;
    Army defender;
    StratObj Objective;

    public Battle(Army atk, Army def) {
        attacker = atk;
        defender = def;
        Objective = null;
    }

    public Battle(Army atk, StratObj def) {
        attacker = atk;
        Objective = def;
        defender = null;
    }

    public void AutoResolve() {
        if (defender != null) { //Is an Army defending
            int atk = Random.Range(attacker.getStrength() / 3, attacker.getStrength());
            int def = Random.Range(defender.getStrength() / 2, defender.getStrength());

            Debug.Log("===== Autoresolving Battle =====" +
                      "\nAttacker: " + attacker.getName() + " Strength: " + attacker.getStrength() +
                      "\nDefender: " + defender.getName() + " Strength: " + defender.getStrength() +
                      "\n" + attacker.getName() + " rolled " + atk +
                      "\n" + defender.getName() + " rolled " + def);

            if (atk > def) { //attacker wins
                /*Rolls for Losses on both sides, winner takes less losses*/
                attacker.AddWin();
                defender.AddLoss();
                defender.Retreat();
                attacker.TakeLosses(Random.Range(0, def / 2)); //Winner
                defender.TakeLosses(Random.Range(0, atk / 3)); //Loser
                
            } else { //defender wins
                defender.AddWin();
                attacker.AddLoss();
                attacker.Retreat();
                attacker.TakeLosses(Random.Range(0, def / 2)); //Loser
                defender.TakeLosses(Random.Range(0, atk / 3)); //Winner                
            }
        } else { //Is an Objective Defending
            if (attacker.getOwnerID() == Objective.getOwnerID()) { return; } //Someone else took it first...           
            int atk = Random.Range(attacker.getStrength() / 3, attacker.getStrength());
            int def = Random.Range(Objective.getStrength() / 2, Objective.getStrength());

            Debug.Log("===== Autoresolving Assault =====" +
                      "\nAttacker: " + attacker.getName() + " Strength: " + attacker.getStrength() +
                      "\nDefender: " + Objective.getName() + " Strength: " + Objective.getStrength() +
                      "\n" + attacker.getName() + " rolled " + atk +
                      "\n" + Objective.getName() + " rolled " + def);

            if (atk > def) { //attacker wins                
                if (def > 0) {
                    attacker.TakeLosses(Random.Range(0, def / 3)); //Winner                    
                }                
                if (Objective.OccupyingArmy != null) {
                    attacker.AddWin();
                    Objective.OccupyingArmy.AddLoss();                   
                    Objective.TakeLosses(Random.Range(0, atk / 3)); //Loser                                      
                    if(Objective.OccupyingArmy != null) //Will be null if it was destroyed by losses
                        Objective.OccupyingArmy.Retreat();
                }
                attacker.Enter(Objective);
            } else { //defender wins
                Objective.OccupyingArmy.AddWin();
                attacker.AddLoss();
                attacker.Retreat();
                Objective.TakeLosses(Random.Range(0, atk / 4)); //Winner
                attacker.TakeLosses(Random.Range(0, def)); //Loser                
            }
        }
    }
}
