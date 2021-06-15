using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Linq;

public class MazeCreatorSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<MazeData>();
    }
    public static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.LEFT:
                return WallState.RIGHT;
            case WallState.RIGHT:
                return WallState.LEFT;
            case WallState.UP:
                return WallState.DOWN;
            case WallState.DOWN:
                return WallState.UP;
            default:
                return WallState.UP;
        }
    }

    private MazeNode[,] ApplyRecursivebackTracker(MazeNode[,] maze, int width, int height)
    {
        var rng = new System.Random();
        var positionStack = new Stack<Position>();
        var position = new Position { X = rng.Next(0, width), Y = rng.Next(0, height) };
        maze[position.X, position.Y].WallState |= WallState.VISITED;
        positionStack.Push(position);

        while (positionStack.Count() > 0)
        {
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbour(current, maze, width, height);

            if (neighbours.Count() > 0)
            {
                positionStack.Push(current);

                var readIndex = rng.Next(0, neighbours.Count());
                var randomNeighbour = neighbours[readIndex];

                var nPosition = randomNeighbour.Position;
                maze[current.X, current.Y].WallState &= ~randomNeighbour.SharedWall;
                maze[nPosition.X, nPosition.Y].WallState &= ~GetOppositeWall(randomNeighbour.SharedWall);

                maze[nPosition.X, nPosition.Y].WallState |= WallState.VISITED;

                positionStack.Push(nPosition);
            }
        }

        return maze;
    }

    protected List<Neighbour> GetUnvisitedNeighbour(Position p, MazeNode[,] maze, int width, int height)
    {
        var list = new List<Neighbour>();
        if (p.X > 0)
        {
            if (!maze[p.X - 1, p.Y].WallState.HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X - 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.LEFT
                });
            }
        }
        if (p.Y > 0)
        {
            if (!maze[p.X, p.Y - 1].WallState.HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y - 1
                    },
                    SharedWall = WallState.DOWN
                });
            }
        }
        if (p.Y < height - 1)
        {
            if (!maze[p.X, p.Y + 1].WallState.HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y + 1
                    },
                    SharedWall = WallState.UP
                });
            }
        }
        if (p.X < width - 1)
        {
            if (!maze[p.X + 1, p.Y].WallState.HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X + 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.RIGHT
                });
            }
        }

        return list;
    }

    protected override void OnUpdate()
    {        
        WallState initialState = WallState.LEFT | WallState.RIGHT | WallState.UP | WallState.DOWN;
        var mazeData = GetSingleton<MazeData>();
        var settings = GetSingleton<MazeSettingsData>();
        
        var width = mazeData.Width;
        var height = mazeData.Height;

        settings.Height = height;
        settings.Width = width;

        SetSingleton(settings);

        var maze = new MazeNode[width, height];
        for (int indexX = 0, positionX = -width / 2; indexX < width; indexX++, positionX++)
        {
            for (int indexY = 0, positionY = -height / 2; indexY < height; indexY++, positionY++)
            {
                maze[indexX, indexY].WallState = initialState;
                maze[indexX, indexY].NodePosition = new Position() { X = indexX, Y = indexY };
                maze[indexX, indexY].NodeCenter = new Vector3(positionX, .25f, positionY);
            }
        }
        maze[0, 0].WallState &= ~WallState.LEFT;
        maze[width - 1, height - 1].WallState &= ~WallState.RIGHT;

        maze = ApplyRecursivebackTracker(maze, width, height);

        foreach (var mazeWall in maze)
        {
            var mazeEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(mazeEntity, new MazeRendererData() { Node = mazeWall});
        }        
        
        EntityManager.DestroyEntity(GetSingletonEntity<MazeData>());
    }
}
