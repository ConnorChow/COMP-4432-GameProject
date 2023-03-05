using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OPS.AntiCheat;
using OPS.AntiCheat.Field;
public interface ECS_Component {}

public class ECS_EntityComponentManagement {
    protected ProtectedInt32 MaxEntities = 100;
    protected ProtectedInt32 ActiveEntities = 0;
    public ProtectedInt32[] entityQueue;
    protected ProtectedInt32[] indexQueue;
    protected Queue<ProtectedInt32> DeadEntities= new Queue<ProtectedInt32>();
    public ECS_EntityComponentManagement(int MaxEntities) {
        this.MaxEntities = MaxEntities;
        entityQueue = new ProtectedInt32[MaxEntities];
        indexQueue = new ProtectedInt32[MaxEntities];
        for (int i = 0; i < this.MaxEntities; i++) {
            DeadEntities.Enqueue(i);
        }
    }
    protected void AddEntity() {
        if (ActiveEntities < MaxEntities) {
            int newEntity = DeadEntities.Dequeue();
            entityQueue[ActiveEntities] = newEntity;
            indexQueue[newEntity] = ActiveEntities;
            ActiveEntities++;
        }
    }
    protected void RemoveEntity(int entity) {
        if (ActiveEntities > 0) {
            DeadEntities.Enqueue(entity);
            ActiveEntities--;
            int RemovedIndex = indexQueue[entity];
            indexQueue[entity] = indexQueue[ActiveEntities];
            entityQueue[RemovedIndex] = entityQueue[RemovedIndex];
        }
    }
    public int GetComponentIndex(int entity) {
        return indexQueue[entity];
    }

    public int GetActiveEntities() {
        return ActiveEntities;
    }
}
