using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatDogEnums
{
    public enum SpeciesType
    {
        Dog,
        Cat
    }

    public enum CreateType
    {
        Building,
        Character
    }
    public enum AssetIdType
    {
        BeagleTower = 1001,
        BorderCollieTower,
        BullTerrierTower,
        GermanShepherdTower,
        DobermanTower,
        SaintBernardTower,
        JapaneseBobtailTower,
        TabbyTower,
        PersianTower,
        RussianBlueTower,
        SiameseTower,
        SphynxTower,
        
        Dog = 2000,
        Beagle,
        BorderCollie,
        BullTerrier,
        GermanShepherd,
        Doberman,
        SaintBernard,
        JapaneseBobtail,
        Tabby,
        Persian,
        RussianBlue,
        Siamese,
        Sphynx,

        EliteBeagle = 3001,
        EliteBorderCollie,
        EliteBullTerrier,
        EliteGermanShepherd,
        EliteDoberman,
        EliteSaintBernard,
        EliteJapaneseBobtail,
        EliteTabby,
        ElitePersian,
        EliteRussianBlue,
        EliteSiamese,
        EliteSphynx,

        None,
    }

    public enum AnimationType
    {
        Idle = 6001,
        Run = 6002,
        Attack = 6003,
        Die = 6004,
    }

    public enum ErrorCode
    {
        SOCKET_ERR = 10000, // ���� ���� ����
        CLIENT_VERSION_MISMATCH = 10001, // Ŭ���̾�Ʈ ���� ����ġ
        UNKNOWN_PACKET_TYPE = 10002, // �� �� ���� ��Ŷ ����
        PACKET_DECODE_ERR = 10003, // ��Ŷ ���ڵ� ����
        PACKET_STRUCTURE_MISMATCH = 10004, // ��Ŷ ���� ����ġ
        MISSING_FIELDS = 10005, // �ʼ� �ʵ� ����
        INVALID_PACKET = 10006, // �߸��� ��Ŷ
        HANDLER_NOT_FOUND = 10007, // �ڵ鷯�� ã�� �� ����

        // Register ���� ����
        DUPLICATE_USER_ID = 10100, // �ߺ��� ����� ID
        INVALID_REGISTER_FORMAT = 10102, // PAYLOAD ���� ����

        // Login ���� ����
        INVALID_CREDENTIALS = 10200, // �߸��� ����� ID �Ǵ� ��й�ȣ
        ALREADY_LOGIN = 10201, // �̹� �α��� �� ����
        INVALID_GOOGLE_TOKEN = 10210, // ���� ��ū ������ ����ġ
        EXPIRED_GOOGLE_TOKEN = 10211, // ���� ��ū ��ȿ�� ����

        // Match ���� ����
        MATCH_NOT_FOUND = 10300, // ��Ī ��븦 ã�� �� ����
        MATCH_TIMEOUT = 10301, // ��Ī ��û �ð� �ʰ�

        // Game Start ���� ����
        GAME_ALREADY_STARTED = 10400, // ������ �̹� ���۵�
        GAME_NOT_READY = 10401, // ���� ���� �غ� �� ��
        GAME_CANCELED = 10402, // ���� ���

        // Building Purchase ���� ����
        BUILDING_INSUFFICIENT_FUNDS = 10500, // �ڱ� ����
        ASSET_NOT_FOUND = 10501, // �����Ϸ��� �ǹ��� ã�� �� ����

        // Unit Spawn ���� ����
        INVALID_ASSET_ID = 10600, // �߸��� ���� ID
        UNIT_INSUFFICIENT_FUNDS = 10601, // �ڱ� ����

        // Attack ���� ����
        UNIT_NOT_FOUND = 10700, // �����Ϸ��� ������ ã�� �� ����
        OPPONENT_NOT_FOUND = 10701, // ��� ������ ã�� �� ����

        // Occupation ���� ����
        CHECKPOINT_NOT_FOUND = 10800, // üũ����Ʈ�� ã�� �� ����
        OCCUPATION_IN_PROGRESS = 10801, // �̹� ���� ��
        DUPLICATE_UNIT_IN_CHECKPOINT = 10802, // �̹� üũ����Ʈ�� ���ִ� ������
        UNOCCUPIED_STATE_CHECKPOINT = 10803, // üũ����Ʈ ������ ����
        NOT_FOUND_UNIT_IN_CHECKPOINT = 10804, // üũ����Ʈ�� ������ ���� �����Դϴ�.

        // Game End ���� ����
        GAME_NOT_ACTIVE = 10900, // ������ Ȱ��ȭ�Ǿ� ���� ����
        INVALID_GAME_STATE = 10901, // �߸��� ���� ����

        // Location ���� ����
        INVALID_LOCATION_DATA = 11000, // �߸��� ��ġ ������
        LOCATION_UPDATE_FAILED = 11001, // ��ġ ������Ʈ ����
        UNOWNED_UNIT = 11002, // ������ �������� ���� ����
        INVALID_TIME = 11003, // �߸��� �ð���
        INVALID_DIRECTION = 11004, // �߸��� ���ݷ� Ÿ��

        // GameSession ���� ����
        GAME_NOT_FOUND = 11100, // ������ �÷������� ������ ã�� ����
        GAME_NOT_IN_PROGRESS = 11101, // ���� �������� ������ �ƴ�
        PLAYER_GAME_DATA_NOT_FOUND = 11102, // ������ ���� �����͸� ã�� ����
        OPPONENT_GAME_DATA_NOT_FOUND = 11103, // ������ ���� �����͸� ã�� ����
        OPPONENT_SOCKET_NOT_FOUND = 11104, // ������ ������ ã�� ����

        // UserSession ���� ����
        USER_NOT_FOUND = 11200, // ������ ã�� ����

        // ���ӿ��� ���� ����
        INVALID_ASSET_TYPE = 11300, // �߸��� ���� Ÿ��

        // Game�� User Session ���� ����
        GAME_SESSION_NOT_FOUND = 11400, // ���� ������ ã�� �� ����
        USER_SESSION_NOT_FOUND = 11401, // ���� ������ ã�� �� ����
    }

}


