using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ECS_Component {

}

public struct ECS_ComponentModifier {
    public int entity;
    public int index;
}

public class ECS_EntityComponentManagement {
    public int MaxEntities = 100;
    public int ActiveEntities = 0;
    public int[] entityQueue;
    public int[] indexQueue;
    Stack<int> DeadEntities= new Stack<int>();
    public ECS_EntityComponentManagement(int MaxEntities) {
        this.MaxEntities = MaxEntities;
        this.entityQueue = new int[MaxEntities];
        this.indexQueue = new int[MaxEntities];
        for (int i = 0; i < this.MaxEntities; i++) {
            this.DeadEntities.Push(i);
        }
    }
    public void AddEntity() {
        if (ActiveEntities < MaxEntities) {
            int newEntity = this.DeadEntities.Pop();
            entityQueue[ActiveEntities] = newEntity;
            indexQueue[newEntity] = ActiveEntities;
            ActiveEntities++;
        }
    }
    public void RemoveEntity(int entity) {
        if (ActiveEntities > 0) {
            DeadEntities.Push(entity);
            ActiveEntities--;
            int RemovedIndex = indexQueue[entity];
            indexQueue[entity] = indexQueue[ActiveEntities];
            entityQueue[RemovedIndex] = entityQueue[RemovedIndex];
        }
    }
    public int GetComponentIndex(int entity) {
        return indexQueue[entity];
    }
}
public class ECS_ComponentSystem {
    int[] entityArray;
    int[] indexArray;


}
