using UnityEngine;
using System.Collections;

public class ZMPlayerAnimationCopy : MonoBehaviour {
	private Animator _animator;

	private bool _isRunning;
	// Use this for initialization
	void Awake () {
		ZMPlayerController.PlayerRunEvent += HandlePlayerRunEvent;

		_animator = GetComponent<Animator>();
		Debug.Log("awake");

		_animator.SetBool ("didBecomeActive", true);
	}

	void HandlePlayerRunEvent (ZMPlayerController playerController, bool isRunning)
	{
		//Debug.Log("Handle the run" + isRunning.ToString());

		//_isRunning = isRunning;
	}
	
	// Update is called once per frame
	void Update () {
		/*bool isSkidding = ((_movementDirection == MovementDirectionState.FACING_LEFT && _velocity.x > 0) ||
		                   (_movementDirection == MovementDirectionState.FACING_RIGHT && _velocity.x < 0));
		bool isSliding = (_velocity.x != 0 && _controlMoveState == ControlMoveState.NEUTRAL);
		
		_animator.SetBool ("isSkidding", isSkidding || isSliding);
		_animator.SetBool ("isGrounded", _controller.isGrounded);
		_animator.SetFloat ("velocityY", _velocity.y);*/
	}

	public void Animate(Sprite frame) {
		GetComponent<SpriteRenderer>().sprite = frame;
	}
}
