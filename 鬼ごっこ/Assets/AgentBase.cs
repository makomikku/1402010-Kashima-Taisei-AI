using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;


public class AgentBase : Agent
{
    [SerializeField]
    protected float angularVelocityCoefficient = 1f; //自身の角速度
    [SerializeField]
    protected float velocityCoefficient = 1f; //自身の速度
    public Transform stage; //エージェントが移動する平面のステージ
    [SerializeField]
    protected List<Transform> otherAgents; //自身以外のエージェント
    [SerializeField]
    protected Rigidbody selfRidigBody; //自身のRigidbody
    [SerializeField]
    float top = 12f; //自身の開始位置のz座標の最大値
    [SerializeField]
    float right = 12f; //自身の開始位置のx座標の最大値
    [SerializeField]
    float bottom = -12f; //自身の開始位置のz座標の最小値
    [SerializeField]
    float left = -12f; //自身の開始位置のx座標の最小値
    [SerializeField]
    float minDistanceBetweenOtherAgentsAtStart = 5f; //自身と他のエージェントとの間の開始位置の最小距離
    protected bool hasTouchedDemon = false; //自身の鬼に捕まった?
    protected bool hasTouchedChild = false; //自身の子を捕まえた?

    //他エージェントと一定距離離れたステージ上のランダムな位置を取得します
    protected Vector3 GetStartPosition(List<Transform> _otherAgents, Transform _stage)
    {
        var pos = new Vector3();
        var stagePos = _stage.position;
        while (true)
        {
            var hasDetect = true;
            pos = stagePos + new Vector3(Random.Range(left, right), 0.5f, Random.Range(bottom, top));
            foreach (var tr in _otherAgents)
            {
                if ((tr.position - pos).sqrMagnitude < minDistanceBetweenOtherAgentsAtStart)
                {
                    hasDetect = false;
                    break;
                }
            }
            if (hasDetect)
            {
                break;
            }
        }
        return pos;

    }
    //ステージ中央付近のランダムな位置を取得します
    protected Vector3 GetLookAtTargetPositionAtStart(Transform _stage)
    {
        var dx = Random.Range(-3f, 3f);
        var dz = Random.Range(-3f, 3f);
        var pos = new Vector3(_stage.position.x + dx, transform.position.y, _stage.position.z + dz); // 高さを維持
        return pos;
    }

    public override void OnEpisodeBegin()
    {
        hasTouchedChild = false;
        hasTouchedDemon = false;

        // 開始位置を設定
        transform.position = GetStartPosition(otherAgents, stage);

        // 向きたい方向を取得
        var lookAtPos = GetLookAtTargetPositionAtStart(stage);
        var direction = (lookAtPos - transform.position).normalized;

        // Y 軸回転のみを適用
        var targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f); // X, Z を固定
    }
}
