using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*A General will be like a General in the total war games, He'll have leadership stats etc.
 Later, he'll have a 'Human' Object, allowing him to deploy to the battlefield as well*/
public class General{
    private string Name;
    public int Leadership; 
    public int Reputation;
    //public List<Traits> traits; //List of traits? 
    public Human human;

    public General(string Name) {
        this.Name = Name;
        Leadership = 0;
        Reputation = 0;
        //traits = ??
        human = null;
    }


    public void setName(string n) { Name = n; }
    public string getName() { return Name; }
	
}
