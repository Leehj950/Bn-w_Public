
using CatDogEnums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SocketManager : TCPSocketManagerBase<SocketManager>
{

    public void RegisterResponse(GamePacket gamePacket)
    {
        Debug.Log("ȸ������ �Ϸ�");
        var popupMessage = UIManager.Instance.ShowPopUI<PopupMessage>();
        popupMessage.ShowMessage("ȸ�������� �Ϸ�Ǿ����ϴ�.");
        UIManager.Instance.ClosePopUpUI("RegisterUIPanel");
    }

    public void MatchNotification(GamePacket gamePacket)
    {
        var response = gamePacket.MatchNotification;
        Debug.Log("��Ī �Ϸ�");

        int port = response.Port;

        Disconnect();

        Init(APIModels.ip, port);

        SocketManager.Instance.Connect(() =>
        {
            if (SocketManager.Instance.isConnected)
            {
                // ���� ���� �� ��û ����
                var authRequest = new C2SAuthRequest();
                authRequest.Token = GameManager.Instance.jwtToken;

                GamePacket packet = new GamePacket();
                packet.AuthRequest = authRequest;
                SocketManager.Instance.Send(packet);
                Debug.Log("Auth request sent!");
            }

        });

    }

    public void AuthResponse(GamePacket gamePacket)
    {
        SceneManager.LoadScene("BattleScene");
    }

    public void GameStartNotification(GamePacket gamePacket)
    {
        var response = gamePacket.GameStartNotification;

        Debug.Log("���� ����");

        Debug.Log(response.Species);

        if (response.Species == "dog")
        {
            BattleManager.Instance.isDog = true;
        }
        else if (response.Species == "cat")
        {
            BattleManager.Instance.isDog = false;
        }

        Debug.Log(BattleManager.Instance.isDog);


        foreach (CreateBtn btn in BattleManager.Instance.createBtns)
        {
            btn.Init();
        }

        CastleManager.Instance.CastleSpawn(BattleManager.Instance.isDog);
    }



    public void PurchaseBuildingResponse(GamePacket gamePacket)
    {
        var response = gamePacket.PurchaseBuildingResponse;

        // �� �ǹ� ����
        BattleManager.Instance.creationSystem.BuyBuilding(response.AssetId);
    }

    public void SpawnUnitResponse(GamePacket gamePacket)
    {
        var response = gamePacket.SpawnUnitResponse;

        // �� ���� ����
        BattleManager.Instance.creationSystem.BuyCharacter(response.AssetId, response.UnitId, response.ToTop);

        if (UIManager.Instance.UIScene is BattleSceneUIPanel)
        {
            BattleSceneUIPanel panel = UIManager.Instance.UIScene as BattleSceneUIPanel;
            panel.UpdataButtonList();
        }
    }

    public void SpawnEnemyUnitNotification(GamePacket gamePacket)
    {
        var response = gamePacket.SpawnEnemyUnitNotification;

        // ��� ���� ����
        BattleManager.Instance.creationSystem.BuyEnemyCharacter(response.AssetId, response.UnitId, response.ToTop);
    }


    public void AddEnemyBuildingNotification(GamePacket gamePacket)
    {
        var response = gamePacket.AddEnemyBuildingNotification;
        // ��� �ǹ� ����
        BattleManager.Instance.creationSystem.BuyEnemyBuilding(response.AssetId);
    }

    public void MineralSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.MineralSyncNotification;
        // ��� ����ȭ
        CurrencyManager.Instance.currency = response.Mineral;
        CurrencyManager.Instance.UpdateCurrencyUI();
    }

    public void LocationSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.LocationSyncNotification;

        // ���� ��ġ ����ȭ
        Google.Protobuf.Collections.RepeatedField<UnitPosition> unitPositions = response.UnitPositions;

        foreach (var item in unitPositions)
        {

            Character character = BattleManager.activeCharacterDic[item.UnitId];

            if (character != null)
            {
                Vector3 targetPos = Vector3.zero;
                Vector3 newEulerAngles = Vector3.zero;
                Quaternion targetRot;

                var positionX = item.Position.X;
                var positionZ = item.Position.Z;

                if (BattleManager.Instance.isDog)
                {
                    targetPos = new Vector3(positionX, character.transform.position.y, positionZ);
                }
                else
                {
                    targetPos = new Vector3(-positionX, character.transform.position.y, positionZ);
                }


                if (character.IsEnemy())
                {
                    float correctedY = item.Rotation.Y;

                    newEulerAngles = new Vector3(
                        character.transform.eulerAngles.x,
                        -correctedY,
                        character.transform.eulerAngles.z
                    );
                }
                else
                {
                    float correctedY = item.Rotation.Y;

                    newEulerAngles = new Vector3(
                        character.transform.eulerAngles.x,
                        correctedY,
                        character.transform.eulerAngles.z
                    );
                }

                targetRot = Quaternion.Euler(newEulerAngles);
                character.smoothMovement.UpdateTargetTransform(targetPos, targetRot);

            }
        }
    }

    public void AttackUnitNotification(GamePacket gamePacket)
    {
        var response = gamePacket.AttackUnitNotification;

        Character character = BattleManager.activeCharacterDic[response.AttackingUnitId];
        character.targetEnemyList.Clear();

        if (response.Success)
        {
            // Ÿ���� ������ ������ȯ
            for (int i = 0; response.TargetUnitIds.Count > i; i++)
            {
                int unitId = response.TargetUnitIds[i];
                Character targetCharacter = BattleManager.activeCharacterDic[unitId];
                character.targetEnemyList.Add(targetCharacter);
            }

            character.targetEnemy = character.targetEnemyList.First();

            // ������ ���ֿ��� Ÿ�ٵ��� �ٽ� �ְ� �������� ��ȯ
            character.ChangeStateByAnimationType(AnimationType.Attack);

        }
        else
        {
            character.ChangeStateByAnimationType(AnimationType.Run);
        }
    }


    public void DamageUnitNotification(GamePacket gamePacket)
    {
        var response = gamePacket.DamageUnitNotification;

        var unitInfo = response.UnitInfo;

        // activeCharacterDic���� �ش� UnitId�� Character ��ü�� �����ɴϴ�.
        if (BattleManager.activeCharacterDic.TryGetValue(unitInfo.UnitId, out var character) && character != null)
        {
            // ĳ������ HP ������Ʈ

            character.characterData.Hp = unitInfo.UnitHp;

            // HP �� ������Ʈ
            float hpPercentage = (float)unitInfo.UnitHp / character.characterData.MaxHp;

            Transform characterTransform = character.transform;

            HPBarManager.Instance.UpdateHPBar(unitInfo.UnitId, characterTransform, hpPercentage);
        }
    }

    public void EnemyUnitAnimationNotification(GamePacket gamePacket)
    {
        var response = gamePacket.EnemyUnitAnimationNotification;

        Character character = BattleManager.activeCharacterDic[response.UnitId];

        character.ChangeStateByAnimationType((AnimationType)response.AnimationId);
    }

    public void ErrorNotification(GamePacket gamePacket)
    {
        var response = gamePacket.ErrorNotification;

        if (response.ErrorCode == (int)ErrorCode.DUPLICATE_USER_ID
            || response.ErrorCode == (int)ErrorCode.INVALID_REGISTER_FORMAT
            || response.ErrorCode == (int)ErrorCode.INVALID_CREDENTIALS
            || response.ErrorCode == (int)ErrorCode.ALREADY_LOGIN
            || response.ErrorCode == (int)ErrorCode.INVALID_GOOGLE_TOKEN
            || response.ErrorCode == (int)ErrorCode.EXPIRED_GOOGLE_TOKEN)
        {
            var popupMessage = UIManager.Instance.ShowPopUI<PopupMessage>();
            popupMessage.ShowMessage(response.ErrorMessage);
        }

        Debug.Log(response.ErrorCode);
        Debug.Log(response.ErrorMessage);
    }

    public void TryOccupationNotification(GamePacket gamePacket)
    {
        var response = gamePacket.TryOccupationNotification;
        bool isTop = response.IsTop; // ������ ��ġ
        bool isOpponent = response.IsOpponent;
        CaptureManager.Instance.OnTryOccupation(isTop, isOpponent);
    }

    public void PauseOccupationNotification(GamePacket gamePacket)
    {
        var response = gamePacket.PauseOccupationNotification;
        bool isTop = response.IsTop;
        CaptureManager.Instance.OnPauseOccupation(isTop);
    }

    public void OccupationTimerResetNotification(GamePacket gamePacket)
    {
        var response = gamePacket.OccupationTimerResetNotification;
        bool isTop = response.IsTop;
        CaptureManager.Instance.OnResetOccupationTimer(isTop);
    }

    public void OccupationSuccessNotification(GamePacket gamePacket)
    {
        var response = gamePacket.OccupationSuccessNotification;
        bool isTop = response.IsTop;
        bool isOpponent = response.IsOpponent;
        CaptureManager.Instance.OnOccupationSuccess(isTop, isOpponent);
    }

    public void UnitDeathNotification(GamePacket gamePacket)
    {
        var response = gamePacket.UnitDeathNotification;

        foreach (var unitId in response.UnitIds)
        {
            Character character = BattleManager.activeCharacterDic[unitId];
            character.characterStateMachine.ChangeState(character.characterStateMachine.dieState);
        }
    }

    public void AttackBaseResponse(GamePacket gamePacket)
    {
        var response = gamePacket.AttackBaseResponse;

        Character character = BattleManager.activeCharacterDic[response.UnitId];

        Debug.Log("�������غ�");
        if (response.Success)
        {
            character.targetBuilding = CastleManager.Instance.enemyCastle;
            character.ChangeStateByAnimationType(AnimationType.Attack);
            Debug.Log("�������غ񼺰�");
            
        }
        else
        {
            Debug.Log("�������غ����");
            character.ChangeStateByAnimationType(AnimationType.Run);
        }
    }

    public void EnemyAttackBaseNotification(GamePacket gamePacket)
    {
        var response = gamePacket.EnemyAttackBaseNotification;

        Character character = BattleManager.activeCharacterDic[response.UnitId];

        if (response.Success)
        {
            character.targetBuilding = CastleManager.Instance.myCastle;
            character.ChangeStateByAnimationType(AnimationType.Attack);
          
        }
        else
        {
            character.ChangeStateByAnimationType(AnimationType.Run);
        }
    }

    public void DamageBaseResponse(GamePacket gamePacket)
    {
        var response = gamePacket.DamageBaseResponse;

        int newHP = response.BaseHp;
        
        DebugOpt.Log($"�� ��ä ���� + {newHP}");

        CastleManager.Instance.UpdateCastleHP(1, newHP);
    }

    public void EnemyDamageBaseNotification(GamePacket gamePacket)
    {
        // ���� ��ü ���ݴ��� 
        Debug.Log("��ü���ݴ���");
        var response = gamePacket.EnemyDamageBaseNotification;

        int newHP = response.BaseHp;
        CastleManager.Instance.UpdateCastleHP(0, newHP);
    }

    public void GameOverNotification(GamePacket gamePacket)
    {
        DebugOpt.Log("��������");

        // ��������� ��Ŷ ���� �ߴ�
        BattleManager.Instance.isGameOver = true;
        SocketManager.Instance.sendQueue.Clear();

        var response = gamePacket.GameOverNotification;

        bool isWin = response.IsWin;

        // ���� �¸� �й� UIǥ���� ���۾��� �κ� ȭ������ ���ư���
        BattleManager.Instance.GameOver(isWin);

    }

    public void HealUnitNotification(GamePacket gamePacket)
    {
        var response = gamePacket.HealUnitNotification;
        Debug.Log(response);
        
        Debug.Log("네이밍" + response.UnitHp);
        Debug.Log("네이밍" + response.UnitId);

        int unitId = response.UnitId;
        int unitHp = response.UnitHp;
        bool succecs = response.Success;

        Debug.Log("힐" + unitId);
        Debug.Log("힐량" + unitHp);
        Debug.Log("힐 리스폰스 성공"+succecs);

        if(succecs)
        {
            SkillManager.Instance.HealTargetUnit(unitId, unitHp);

            Character character = BattleManager.Instance.GetCharacterById(unitId);

            float hpPercentage = (float)unitHp / character.characterData.MaxHp;

            Transform characterTransform = character.transform;

            HPBarManager.Instance.UpdateHPBar(unitId, characterTransform, hpPercentage);
        }
    }

    public void BuffUnitNotification(GamePacket gamePacket)
    {
        var response = gamePacket.BuffUnitNotification;

        List<int> units = response.UnitIds.ToList();
        int buffAmount = response.BuffAmount;
        int buffDuration = response.BuffDuration;
        bool succecs = response.Success;

        if (succecs)
        SkillManager.Instance.ApplyAttackSpeedBuffFromServer(units, buffAmount, buffDuration);
    }

    public void DrawCardResponse(GamePacket gamePacket)
    {
        var response = gamePacket.DrawCardResponse;

        BattleManager.Instance.AssetidList.Add(response.AssetId);

        if (UIManager.Instance.UIScene is BattleSceneUIPanel)
        {
            var panel = UIManager.Instance.UIScene as BattleSceneUIPanel;
            panel.UpdataButtonList();
        }
    }

    public void EliteCardNotification(GamePacket gamePacket)
    {
        var response = gamePacket.EliteCardNotification;

        BattleManager.Instance.AssetidList.RemoveAll(id => id == response.ConsumedAssetId);
        BattleManager.Instance.AssetidList.Add(response.EliteAssetId);

        if (UIManager.Instance.UIScene is BattleSceneUIPanel)
        {
            var panel = UIManager.Instance.UIScene as BattleSceneUIPanel;
            panel.UpdataButtonList();
        }
    }
}