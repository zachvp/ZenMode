using UnityEngine;
using System.Collections.Generic;
using ZMPlayer;

public class ZMMetricsCollector : MonoBehaviour
{
	private ZMPlayerInfo _playerInfo;

	private enum MetricsType { POSITION, JUMP, WALL_JUMP, DEATH, WARP, LUNGE, PLUNGE };

	private List<Vector3> _positionData 	= new List<Vector3>();
	private List<Vector3> _jumpData 		= new List<Vector3>();
	private List<DeathMetric> _deathData 	= new List<DeathMetric>();

	// Switches
	private bool _shouldDrawPositionData;
	private bool _shouldDrawJumpData;
	private bool _shouldDrawDeathData;

	// Colors
	private Color _drawPosColor = new Color(255, 255, 255, 0.2f);
	private Color _drawJumpColor = new Color(0, 255, 0, 0.2f);

	// Constants
	private const float kAddPositionInterval = 0.5f;

	// Delegates
	public delegate void MetricsAddPositionAction(int player, Vector3 position);
	public static MetricsAddPositionAction MetricsAddPositionEvent;

	// Use this for initialization
	void Start () {
		InvokeRepeating("AddPosition", 0.001f, kAddPositionInterval);

		_playerInfo = GetComponent<ZMPlayerInfo>();
	}

	void OnDestroy() {
		MetricsAddPositionEvent = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Backspace)) {
			_shouldDrawPositionData = !_shouldDrawPositionData;
			_shouldDrawJumpData = !_shouldDrawJumpData;
			_shouldDrawDeathData = !_shouldDrawDeathData;
		}
	}

	void OnDrawGizmos() {
		if (_shouldDrawPositionData) {
			DrawData(_positionData, _drawPosColor);
		}

		if (_shouldDrawJumpData) {
			DrawData (_jumpData, _drawJumpColor);
		}

		if (_shouldDrawDeathData) {
			DrawDeathData();
		}
	}

	// Public methods
	public void AddJumpData(ZMPlayerInputController inputController) {
		//Debug.Log (gameObject.name + ": add jump data");

//		_jumpData.Add(gameObject.transform.position);
	}

	public void AddDeathData(int type) {
		_deathData.Add(new DeathMetric(gameObject.transform.position, type));
	}

	// Private methods
	private void AddPosition() {
		_positionData.Add(gameObject.transform.position);

		if (MetricsAddPositionEvent != null) {
			MetricsAddPositionEvent(_playerInfo.ID, gameObject.transform.position);
		}
	}

	private void DrawDeathData(DeathMetric metric) {
		if (metric.type == 0) {
			DrawDataPoint(metric.position, Color.red);
		} else {
			DrawDataPoint(metric.position, Color.green);
		}
	}

	private void DrawDeathData() {
		foreach (DeathMetric metric in _deathData) {
			DrawDeathData(metric);
		}
	}

	private void DrawData(List<Vector3> dataSet, Color dataColor) {
		foreach (Vector3 position in dataSet) {
			DrawDataPoint(position, dataColor);
		}
	}

	private void DrawDataPoint(Vector3 point, Color color) {
		Gizmos.color = color;
		Gizmos.DrawSphere(point, 12.0f);
	}
}

struct DeathMetric {
	public Vector3 position;
	public int type;

	public DeathMetric(Vector3 pos, int type) {
		position = pos;
		this.type = type;
	}
};
