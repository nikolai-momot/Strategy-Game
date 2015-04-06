using UnityEngine;
using System.Collections;

public class SeatTypes : MonoBehaviour {
	public static string[][] SeatList = new string[][]{
		new string[]{"driver","gunner","commander","charger","gunner2","seat1","seat2","seat3","seat4","seat5","seat6","seat7","seat8","seat9","seat10"}, //Tank
		new string[]{"gunner","commander","commander1","commander2","seat1","seat2","seat3","seat4","seat5","seat6"}, //Gun
		new string[]{"driver","commander","seat1","seat2","seat3","seat4","seat5","seat6","seat7","seat8","seat9","seat10"}, //Truck
		new string[]{"driver","gunner","seat1","seat2","seat3","seat4","seat5","seat6","seat7","seat8","seat9","seat10"}, //Transport
	};
}
