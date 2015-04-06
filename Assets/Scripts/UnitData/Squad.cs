using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad{
	string name;
	Dictionary<string,Human> members;

	public Squad(string name){
		this.name = name;
		members = new Dictionary<string,Human>();
	}

	public override string ToString ()
	{
		string squad = "{Squad\n \t{Name \"" + this.name + "\"}\n\t{Actors "; //{Squad	{Name "squad"} {Actors 
		foreach (Human member in members.Values) {
			squad += " " + member.getMID() + " ";
		}
		squad += "}\n}";
		return squad;
	}

	public int CountMembers(){
		return members.Count;
	}

	public void setName(string n){this.name = n;}
	public string getName(){return this.name;}

	public void AddMember(Human newGuy){
		members.Add (newGuy.getHID(),newGuy);
	}
	public void RemoveMember(Human guy){
		members.Remove (guy.getHID ());
	}
}



/*{Squads
		{Squad
			{Name "squadexample"}
			{Id 0}
			{Actors 10 13 14 15 16 17 18}
		}
}*/