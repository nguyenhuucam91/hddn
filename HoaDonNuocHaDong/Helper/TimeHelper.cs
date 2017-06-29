using System;
class TimeHelper
{
    public static String getThangTrongQuy(int quy)
    {
        if (quy < 0)
        {
            throw new Exception("Quý không được âm");
        }
        else
        {
            switch (quy)
            {
                case 1: return "1,2,3";
                case 2: return "4,5,6";
                case 3: return "7,8,9";
                case 4: return "10,11,12";
                default: return "0";
            }
        }
    }

    public static int getQuyHienTai(int thang)
    {
        switch (thang)
        {
            case 1:
            case 2:
            case 3:
                return 1;
            case 4:
            case 5:
            case 6:
                return 2;
            case 7:
            case 8:
            case 9:
                return 3;
            case 10:
            case 11:
            case 12:
                return 4;
        }
        return 0;
    }
}