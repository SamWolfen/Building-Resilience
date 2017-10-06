using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceProducer : MonoBehaviour {

	/// Amount of resource currently available.
	public float resourceAmount;

	/// Production rate for resource.
	public float resourceProductionRate;

	/// Harvest rate for resource - proportion of maximum consumed per second.
	public float harvestRate;

	/// Capacity of resource producer - maximum amount that can be present.
	public float maximumResourceLevel;

	/// Remove this producer when the resource level reaches zero.
	public bool removeWhenEmpty;

	/// The index identifying the particular resource type as defined by GloopResources.
	public int resourceType;

	/// Link to details about various resource types.
	private GameObject gameGlobals = null;

	private void updateResourceLevels ()
	{
		if (resourceAmount > maximumResourceLevel)
		{
			resourceAmount = maximumResourceLevel;
		}
		if (resourceAmount <= 0.0f)
		{
			resourceAmount = 0.0f;
			if (removeWhenEmpty)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}

		if (gameGlobals == null)
		{
			gameGlobals = GameObject.Find("GameGlobals");

			//THIS LINE THROWS ERRORS. FIX THIS.
			if (this.resourceType == 0 || this.resourceType == 1 || this.resourceType == 2 || this.resourceType == 3)
			{
				//gameObject.GetComponent<MeshRenderer>().material = gameGlobals.GetComponent<GloopResources>().resourceMaterials[resourceType]; //THIS LINE THROWS ERRORS. FIX THIS.
			}
			//THIS LINE THROWS ERRORS. FIX THIS.
		}
		if (gameGlobals != null)
		{
			Color c = gameObject.GetComponent<MeshRenderer>().material.color;
			c.a = resourceAmount;
			gameObject.GetComponent<MeshRenderer>().material.color = c;
		}
	}

	void OnTriggerStay (Collider collision)
	{
		var hit = collision.transform.parent.gameObject; // if hitting a player shape, get the player (parent).
		var playerState = hit.GetComponent<PlayerState>();
		if (playerState != null)
		{
			float amountHarvested = harvestRate * Time.deltaTime;
			amountHarvested = Math.Min (resourceAmount, amountHarvested);
			playerState.changeResource (resourceType, amountHarvested);

			// Note: resource depleted even if the player doesn't benefit. Reduce
			// the amount harvested by the amount the player has taken if there is
			// a need to change this behaviour.

			resourceAmount -= amountHarvested;
			updateResourceLevels ();
		}
	}

	void Update() 
	{
		//resourceAmount += resourceProductionRate * Time.deltaTime;
		//updateResourceLevels ();
	}
}
