﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FixedObjectPositioningMap
{
    private Queue<ETileTypes> _listTiles = new Queue<ETileTypes>();

    private int _lastPositioningIndex = 0;
    private int _creatablePosition = 0;
    private int _randomNumber = 0;

    private const int CREATABLE_BIT_NUMBER = 256;
    private const int TOTAL_CREATABLE_POSITION_INDEX = 3;

    public int CreatablePosition { get { return _creatablePosition; } }

    public void CreateFixedObstaclePosition()
    {
        _randomNumber = UnityEngine.Random.Range(0, CREATABLE_BIT_NUMBER);

        CreateRandomNumber(_randomNumber);

        CreateRandomPosition(_randomNumber, _lastPositioningIndex);
    }

    private void CreateRandomNumber(int num)
    {
        int count = 0;
        int temp = 1;

        for(int i = 0; i < ConstantValue.MAX_FIXED_OBSTACLE_POSITION_INDEX; ++i)
        {
            if (0 != (num & temp))
            {
                ++count;

                if (count > TOTAL_CREATABLE_POSITION_INDEX)
                {
                    _randomNumber = UnityEngine.Random.Range(0, CREATABLE_BIT_NUMBER);

                    CreateRandomNumber(_randomNumber);
                    return;
                }
            }

            temp <<= 1;
        }
    }

    private void CreateRandomPosition(int lhs, int rhs)
    {
        int temp = 1;

        for (int i = 0; i < ConstantValue.MAX_FIXED_OBSTACLE_POSITION_INDEX; ++i)
        {
            if (0 == ((lhs & temp) | (rhs & temp)))
            {
                _lastPositioningIndex = lhs | rhs;

                _creatablePosition = lhs;

                return;
            }

            temp <<= 1;
        }

        CreateFixedObstaclePosition();
    }
}
