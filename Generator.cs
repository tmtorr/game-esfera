using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    
    //the enum for structures, so they can be used with more descriptive names
    enum structures{
        floor, //0
        smallRoom, //1
        largeRoom, //2
        pillar //3
    }


    private int generatedCoordinate_x;
    private int generatedCoordinate_z;
    private int generatedAngle = 0; //set for default

    int r;



    public GameObject[] gameObjects; //referenced in Unity

    void Start()
    {
        int i, j;

        //generate via chunks, 5x5 chunks, each 6m squared
        for(i = -2; i < 3; i++)
        {
            for(j = -2; j < 3; j++)
            {   
                if(i != 0 && j != 0){
                    GenerateStructure(i*6, j*6, 0);
                //numbers are increments 6 so that the floors and structures don't overlap during placement
                }
            }
        }
    }


    /// <summary>
    /// Generates concrete structures
    /// </summary>
    /// <param name="x_coord"> x coordinate</param>
    /// <param name="z_coord"> y coordinate </param>
    /// <param name="heightLevel"> 1 in 6 chance of increasing</param>
    void GenerateStructure(int x_coord, int z_coord, int heightLevel)
    {
        r = Random.Range(0,3);
        if(r == 0 || r == 1) //2 in 3 chance to generate anything all, 66% chance
        {
            //generates floor level, then 1 in 4 chance to generate subsequent higher levels
            //was going to do it recursively, but adding to the stack was less efficient
            do
            {
                //generates floor
                GenerateObject(x_coord, z_coord, structures.floor, heightLevel);

                r = Random.Range(0,4); //r number between 0 and 3
                if(r%2 == 1){ //if r is even; 1/2 chance generate small room
                    GenerateObject(x_coord, z_coord, structures.smallRoom, heightLevel);
                }
                else{ //r is odd; 1/2 chance generate large room
                    GenerateObject(x_coord, z_coord, structures.largeRoom, heightLevel);
                }

                heightLevel += 1;    

            }while(r == 1); //1/4 chance generate second level    
        }

    }


    /// <summary>
    /// Generates objects across the map using Random.Range
    /// </summary>
    /// <param name="objectType"> 
    ///     0 = floor,
    ///     1 = small room,
    ///     2 = large room,
    ///     3 = pillar
    /// </param>
    /// 
    /// <param name="heightLevel"> 
    ///     0 = ground level,
    ///     1 = 2nd floor,
    ///     2 = 3rd,
    ///     3 = 4th... and so on..
    /// </param>
    /// 
    void GenerateObject(int x, int z, structures objectType, int heightLevel)
    {
        Vector3 generatedCoordinates;
        generatedCoordinates = new Vector3(x, (float)(heightLevel * 2.5), z);

        //if structure not floor spawn within 6x6 grid
        if(objectType == structures.smallRoom) 
        {
            x = Random.Range(-2, 3) + x;
            z = Random.Range(-2, 3) + z;
            generatedAngle = Random.Range(0, 5) * 90;
            //possible angles are 0, 90, 180, 270 and 360
            //0*90=0 | 1*90=90 | 2*90=180 | 3*90=270 | 4*90=360

            generatedCoordinates = new Vector3(x, (float)((heightLevel * 2.5) + 0.1), z);
        }

        else if(objectType == structures.largeRoom)
        {
            //angle calculated first because it will affect how well the room will fit
            generatedAngle = Random.Range(0, 5) * 90;

            if(generatedAngle == 90 || generatedAngle == 270){ //z can change but x must be static 
                z = Random.Range(-2, 3) + z;
            }

            else{ //x must be static but z can change if angles are 0 and 180
                x = Random.Range(-2, 3) + x;
            }

            generatedCoordinates = new Vector3(x, (float)((heightLevel * 2.5) + 0.1), z);
        }

        else if(objectType == structures.floor && heightLevel > 0)
        //Longer and more complex because 4 pillars are erected below the floor if it's above 0 stories
        {
            Vector3 pillarCoord1 = new Vector3((float)(x + 2.5), (float)((heightLevel*2.5) - 1.2), (float)(z + 2.5));
            Vector3 pillarCoord2 = new Vector3((float)(x + 2.5), (float)((heightLevel*2.5) - 1.2), (float)(z - 2.5));
            Vector3 pillarCoord3 = new Vector3((float)(x - 2.5), (float)((heightLevel*2.5) - 1.2), (float)(z - 2.5));
            Vector3 pillarCoord4 = new Vector3((float)(x - 2.5), (float)((heightLevel*2.5) - 1.2), (float)(z + 2.5));
            Instantiate(gameObjects[(int)structures.pillar], pillarCoord1, Quaternion.Euler(0, 0, 0));//creates pillar
            Instantiate(gameObjects[(int)structures.pillar], pillarCoord2, Quaternion.Euler(0, 0, 0));//creates pillar
            Instantiate(gameObjects[(int)structures.pillar], pillarCoord3, Quaternion.Euler(0, 0, 0));//creates pillar
            Instantiate(gameObjects[(int)structures.pillar], pillarCoord4, Quaternion.Euler(0, 0, 0));//creates pillar
            //game grid is -20 to 20 on x and z, each floor in the bulding is 2.5 meters 
        }

        Quaternion angle = Quaternion.Euler(0, generatedAngle, 0);//sets the angle

        Instantiate(gameObjects[(int)objectType], generatedCoordinates, angle);//creates the object
    }
}
