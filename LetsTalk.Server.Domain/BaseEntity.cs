﻿namespace LetsTalk.Server.Domain;

public class BaseEntity
{
    public int Id { get; protected set; }

    public BaseEntity(int id)
    {
        Id = id;
    }

    protected BaseEntity()
    {
    }
}
