using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexmap : MonoBehaviour {


	void Start ()
    {
        GenerateMap();
	}

    public GameObject HexPrefab;

	public void GenerateMap ()
    {
        for (int col = 0; col < 10; col++)
        {
            for (int row = 0; row < 10; row++)
            {
                // Instantiate a Hex
                Hex h = new Hex(col, row);
                Instantiate(HexPrefab, h.Position(), Quaternion.identity, this.transform);
            }
        }
    }
}
