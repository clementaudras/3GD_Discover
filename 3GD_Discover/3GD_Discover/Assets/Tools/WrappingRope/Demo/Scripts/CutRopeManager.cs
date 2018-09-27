using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WrappingRopeLibrary.Enums;
using WrappingRopeLibrary.Events;
using WrappingRopeLibrary.Scripts;

public class CutRopeManager : MonoBehaviour {

    public GameObject RopeInstance;

    public GameObject RopeInstance1;

    public float MinYForDestroy;
    private Rope _sourceRope;

    private List<Rope> _ropes;

    private bool _expandRope;

	// Use this for initialization
	void Start () {
        _ropes = new List<Rope>();
	    if (RopeInstance != null)
        {
            _sourceRope = RopeInstance.GetComponent<Rope>();
            _sourceRope.ObjectWrap += Rope_ObjectWrapping;
        }
    }

    public void Rope_ObjectWrapping(RopeBase sender, ObjectWrapEventArgs args)
    {
        if (args.WrapPoints.Length == 0)
            return;

        var newRope = Instantiate(RopeInstance1).GetComponent<Rope>();
        _ropes.Add(newRope);

        // Don't want to wrap
        args.Cancel = true;

        // Set new rope ends
        newRope.FrontEnd.transform.position = args.WrapPoints[0];
        newRope.BackEnd.transform.position = _sourceRope.BackEnd.transform.position;
        // Refresh piece ends positions by rope ends from previous two lines. The rope has only one piece. FrontBandPoint corresponds the FrontEnd. BackBandPoint corresponds the BackEnd.
        newRope.FrontPiece.FrontBandPoint.SetPointInWorldSpace(0f);
        newRope.FrontPiece.BackBandPoint.SetPointInWorldSpace(0f);
        // Call the method below to recalculate length 
        newRope.FrontPiece.Relocate();
        // Setting Anchoring mode will constrain current rope length
        newRope.AnchoringMode = AnchoringMode.ByFrontEnd;

        // Temporary off anchoring mode
        _sourceRope.AnchoringMode = AnchoringMode.None;
        // Use native function to shrink the rope
        _sourceRope.CutRopeNotAnchoring((args.WrapPoints[0] - _sourceRope.BackEnd.transform.position).magnitude, Direction.BackToFront);
        // Need to call this method to reset previouse position of piece and stabilize the rope
        _sourceRope.FrontPiece.Relocate();
        // Set anchoring mode to accept new length of rope   
        _sourceRope.AnchoringMode = AnchoringMode.ByFrontEnd;

        var projectile = args.Target.GetComponent<Rigidbody>();
        var projectileVeloc = projectile.GetPointVelocity(args.Target.transform.position).magnitude * (args.WrapPoints[0] - args.Target.transform.position);
        var weigh = _sourceRope.BackEnd.GetComponent<Rigidbody>();
        weigh.AddForce(projectileVeloc * 2, ForceMode.VelocityChange);
        weigh = newRope.FrontEnd.GetComponent<Rigidbody>();
        weigh.AddForce(projectileVeloc * 4, ForceMode.VelocityChange);
    }


    // Update is called once per frame
    void Update ()
    {
        ManageInput();
        ManageOutRopes();
    }

    private void ManageInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _expandRope = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            _expandRope = false;
        }
    }

    private void ManageOutRopes()
    {
        var ropesForDestroy = new List<Rope>();
        for (var i = 0; i < _ropes.Count; i++)
        {
            var rope = _ropes[i];
            if (rope.FrontEnd.transform.position.y < MinYForDestroy && rope.BackEnd.transform.position.y < MinYForDestroy)
                ropesForDestroy.Add(rope);
        }
        foreach (var rope in ropesForDestroy)
        {
            _ropes.Remove(rope);
        }
        for (var i = 0; i < ropesForDestroy.Count; i++)
        {
            Destroy(ropesForDestroy[i].gameObject);
        }
    }

    void FixedUpdate()
    {
        if (_expandRope)
        {
            _sourceRope.CutRope(-1f * Time.fixedDeltaTime, Direction.FrontToBack);
        }
    }


}
