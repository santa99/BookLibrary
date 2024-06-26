﻿using Contracts;
using Contracts.Models;

namespace DataAccess;

/// <summary>
/// Dummy implementation of the <see cref="IReadersInfoDao"/>.
/// </summary>
public class ReadersInfoDaoImpl : IReadersInfoDao
{
    private readonly Dictionary<int, Tuple<string, string>> _readersInfo = new()
    {
        { 1, new Tuple<string, string>("Marek", "Blanar") },
        { 2, new Tuple<string, string>("Ivan", "Maly") },
        { 3, new Tuple<string, string>("Tibor", "Vesely") }
    };

    public ReadersInfo? Read(int readersCardId)
    {
        if (!_readersInfo.TryGetValue(readersCardId, out var reader))
        {
            return null;
        }

        return new ReadersInfo
        {
            ReaderCardId = readersCardId,
            FirstName = reader.Item1,
            LastName = reader.Item2
        };
    }

    public List<ReadersInfo> SelectReadersInfos()
    {
        return _readersInfo.Select(pair => new ReadersInfo()
        {
            ReaderCardId = pair.Key,
            FirstName = pair.Value.Item1,
            LastName = pair.Value.Item2
        }).ToList();
    }
}