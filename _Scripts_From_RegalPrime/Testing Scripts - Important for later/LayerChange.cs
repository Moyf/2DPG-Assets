using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct OnePanel
{
	public GameObject baseGameObject;
	public Collider2D[] AllColliders;
	public SpriteRenderer[] AllSpriteRenderers;
}

public class LayerChange : MonoBehaviour
{
	public List<GameObject> PanelsToSwap;
	public GameObject parentGameObject;

	public float DistanceBetweenPanels = 0.32f;

	public int CurrentActivePanel = 0;
	public int MaxPanelIndex = -1;

	private List<OnePanel> PanelComponents;
	private float OffsetCounter = 0f;

	private float GetVerticalInput = 0;
//	private float DelayToSwapPanels = 1f;
	private bool CanISwapPanels = true;

	private ObjectManipulation objectMover;

	void Start()
	{
		PanelComponents = new List<OnePanel> ();
		objectMover = gameObject.AddComponent<ObjectManipulation> ();

		foreach(GameObject GO in PanelsToSwap)
		{
			OnePanel temp;
			temp.baseGameObject = GO;
			temp.AllColliders = GO.GetComponentsInChildren<Collider2D>();
			temp.AllSpriteRenderers = GO.GetComponentsInChildren<SpriteRenderer>();

			PanelComponents.Add (temp);

//			GO.transform.position = (Vector3)(Vector2)GO.transform.position + new Vector3(0,0,OffsetCounter);
			GO.transform.localPosition = new Vector3(0,0,DistanceBetweenPanels*(MaxPanelIndex+1));

			OffsetCounter += DistanceBetweenPanels;

			MaxPanelIndex++;
			if(MaxPanelIndex != 0)
			{
				SetAllColliders (PanelComponents[MaxPanelIndex], false);
				SetSpriteAlpha  (PanelComponents[MaxPanelIndex], 0.5f);
			}
				
		}

		CurrentActivePanel = 0;

//		foreach(OnePanel aPanel in PanelComponents)
//		{
//			print (string.Format ("{0}.  Colliders = {1}.  SpriteRenderers = {2}", aPanel.baseGameObject, aPanel.AllColliders.Length, aPanel.AllSpriteRenderers.Length));
//
//		}
	}

	void Update()
	{
		if(CanISwapPanels)
		{
			GetVerticalInput = Input.GetAxis("Vertical");

			if(GetVerticalInput > 0)
			{
				StartCoroutine (MoveForwardAPanel());
				CanISwapPanels = false;
			}

			if(GetVerticalInput < 0)
			{
				StartCoroutine (MoveBackwardsAPanel());
				CanISwapPanels = false;
			}
		}


	}


	IEnumerator MoveForwardAPanel()
	{
		if(CurrentActivePanel+1 <= MaxPanelIndex)
		{
			SetAllColliders (PanelComponents[CurrentActivePanel], false);
			SetSpriteAlpha  (PanelComponents[CurrentActivePanel], 0.5f);

			CurrentActivePanel++;

			SetAllColliders (PanelComponents[CurrentActivePanel], true);
			SetSpriteAlpha  (PanelComponents[CurrentActivePanel], 1.0f);


			objectMover.MoveObjectToPoint(0, parentGameObject, (parentGameObject.transform.position + new Vector3(0,0,-DistanceBetweenPanels)), 0.25f);
//			parentGameObject.transform.position += new Vector3(0,0,-DistanceBetweenPanels);
		}

		yield return new WaitForSeconds (1);
		CanISwapPanels = true;
	}
	IEnumerator MoveBackwardsAPanel()
	{
		if(CurrentActivePanel-1 >= 0)
		{
			SetAllColliders (PanelComponents[CurrentActivePanel], false);
			SetSpriteAlpha  (PanelComponents[CurrentActivePanel], 0.5f);

			CurrentActivePanel--;

			SetAllColliders (PanelComponents[CurrentActivePanel], true);
			SetSpriteAlpha  (PanelComponents[CurrentActivePanel], 1.0f);


			objectMover.MoveObjectToPoint(0, parentGameObject, (parentGameObject.transform.position + new Vector3(0,0,DistanceBetweenPanels)), 0.25f);
//			parentGameObject.transform.position += new Vector3(0,0,DistanceBetweenPanels);
		}		

		yield return new WaitForSeconds (1);
		CanISwapPanels = true;
	}


	public void SetAllColliders(OnePanel aPanel, bool isEnabled)
	{
		foreach(Collider2D aCollider in aPanel.AllColliders)
		{
			aCollider.enabled = isEnabled;
		}
	}

	public void SetSpriteAlpha(OnePanel aPanel, float AlphaValue)
	{
		foreach(SpriteRenderer aSpriteRenderer in aPanel.AllSpriteRenderers)
		{
			objectMover.ChangeAlpha(0, aSpriteRenderer.gameObject, AlphaValue, 1);
		}
	}

}



















/*

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct OnePanel
{
	public GameObject baseGameObject;
	public Collider2D[] AllColliders;
	public SpriteRenderer[] AllSpriteRenderers;
}

public class LayerChange : MonoBehaviour
{
	public List<GameObject> PanelsToSwap;
	public GameObject parentGameObject;

	public float DistanceBetweenPanels = 1f;


	public int CurrentActivePanel = 0;
	public int MaxPanelIndex = -1;

	private List<OnePanel> PanelComponents;
	private float OffsetCounter = 0f;

	private float GetVerticalInput = 0;
	private float DelayToSwapPanels = 1f;
	private bool CanISwapPanels = true;

	private ObjectManipulation objectMover;

	void Start()
	{
		PanelComponents = new List<OnePanel> ();
		objectMover = gameObject.AddComponent<ObjectManipulation> ();

		foreach(GameObject GO in PanelsToSwap)
		{
			OnePanel temp;
			temp.baseGameObject = GO;
			temp.AllColliders = GO.GetComponentsInChildren<Collider2D>();
			temp.AllSpriteRenderers = GO.GetComponentsInChildren<SpriteRenderer>();

			PanelComponents.Add (temp);

//			GO.transform.position = (Vector3)(Vector2)GO.transform.position + new Vector3(0,0,OffsetCounter);
			GO.transform.localPosition = new Vector3(0,0,1*MaxPanelIndex+1);

			OffsetCounter += DistanceBetweenPanels;

			MaxPanelIndex++;
			if(MaxPanelIndex != 0)
			{
				SetAllColliders (PanelComponents[MaxPanelIndex], false);
				SetSpriteAlpha  (PanelComponents[MaxPanelIndex], 0.5f);
			}
				
		}

		CurrentActivePanel = 0;

//		foreach(OnePanel aPanel in PanelComponents)
//		{
//			print (string.Format ("{0}.  Colliders = {1}.  SpriteRenderers = {2}", aPanel.baseGameObject, aPanel.AllColliders.Length, aPanel.AllSpriteRenderers.Length));
//
//		}
	}

	void Update()
	{
		if(CanISwapPanels)
		{
			GetVerticalInput = Input.GetAxis("Vertical");

			if(GetVerticalInput > 0)
			{
				StartCoroutine (MoveForwardAPanel());
				CanISwapPanels = false;
			}

			if(GetVerticalInput < 0)
			{
				StartCoroutine (MoveBackwardsAPanel());
				CanISwapPanels = false;
			}
		}


	}


	IEnumerator MoveForwardAPanel()
	{
		if(CurrentActivePanel+1 <= MaxPanelIndex)
		{
			SetAllColliders (PanelComponents[CurrentActivePanel], false);
			SetSpriteAlpha  (PanelComponents[CurrentActivePanel], 0.5f);

			CurrentActivePanel++;

			SetAllColliders (PanelComponents[CurrentActivePanel], true);
			SetSpriteAlpha  (PanelComponents[CurrentActivePanel], 1.0f);


			objectMover.MoveObjectToPoint(0, parentGameObject, (parentGameObject.transform.position + new Vector3(0,0,-DistanceBetweenPanels)), 1);
//			parentGameObject.transform.position += new Vector3(0,0,-DistanceBetweenPanels);
		}

		yield return new WaitForSeconds (1);
		CanISwapPanels = true;
	}
	IEnumerator MoveBackwardsAPanel()
	{
		if(CurrentActivePanel-1 >= 0)
		{
			SetAllColliders (PanelComponents[CurrentActivePanel], false);
			SetSpriteAlpha  (PanelComponents[CurrentActivePanel], 0.5f);

			CurrentActivePanel--;

			SetAllColliders (PanelComponents[CurrentActivePanel], true);
			SetSpriteAlpha  (PanelComponents[CurrentActivePanel], 1.0f);


			objectMover.MoveObjectToPoint(0, parentGameObject, (parentGameObject.transform.position + new Vector3(0,0,DistanceBetweenPanels)), 1);
//			parentGameObject.transform.position += new Vector3(0,0,DistanceBetweenPanels);
		}		

		yield return new WaitForSeconds (1);
		CanISwapPanels = true;
	}


	public void SetAllColliders(OnePanel aPanel, bool isEnabled)
	{
		foreach(Collider2D aCollider in aPanel.AllColliders)
		{
			aCollider.enabled = isEnabled;
		}
	}

	public void SetSpriteAlpha(OnePanel aPanel, float AlphaValue)
	{
		foreach(SpriteRenderer aSpriteRenderer in aPanel.AllSpriteRenderers)
		{
			objectMover.ChangeAlpha(0, aSpriteRenderer.gameObject, AlphaValue, 1);
		}
	}

}




















*/