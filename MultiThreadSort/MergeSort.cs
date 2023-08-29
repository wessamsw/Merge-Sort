using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Threading;


namespace MultiThreadSort
{
    public class MergeSort
    {
        #region Helper Functions [TASK 1]
        public static object Params2Object(int[] A, int s, int e, int m, int node_idx)
        {
            ArrayList parameters = new ArrayList();
            parameters.Add(A);
            parameters.Add(s);
            parameters.Add(e);
            parameters.Add(m);
            parameters.Add(node_idx);
            return parameters;
        }

        public static void Object2Params(object parameters, ref int[] A, ref int s, ref int e, ref int m, ref int node_idx)
        {
            ArrayList paramList = (ArrayList)parameters;
            A = (int[])paramList[0];
            s = (int)paramList[1];
            e = (int)paramList[2];
            m = (int)paramList[3];
            node_idx = (int)paramList[4];
        }
        #endregion

        //DO NOT CHANGE THIS CODE
        #region Sequential Sort 

        public static void Sort(int[] array)
        {
            MSort(array, 1, array.Length);
        }

        private static void MSort(int[] A, int s, int e)
        {
            if (s >= e)
            {
                return;
            }

            int m = (s + e) / 2;

            MSort(A, s, m);

            MSort(A, m + 1, e);

            Merge(A, s, m, e);
        }

        private static void Merge(int[] A, int s, int m, int e)
        {
            int leftCapacity = m - s + 1;

            int rightCapacity = e - m;

            int leftIndex = 0;

            int rightIndex = 0;

            int[] Left = new int[leftCapacity];

            int[] Right = new int[rightCapacity];

            for (int i = 0; i < Left.Length; i++)
            {
                Left[i] = A[s + i - 1];
            }

            for (int j = 0; j < Right.Length; j++)
            {
                Right[j] = A[m + j];
            }

            for (int k = s; k <= e; k++)
            {
                if (leftIndex < leftCapacity && rightIndex < rightCapacity)
                {
                    if (Left[leftIndex] < Right[rightIndex])
                    {
                        A[k - 1] = Left[leftIndex++];
                    }
                    else
                    {
                        A[k - 1] = Right[rightIndex++];
                    }
                }
                else if (leftIndex < leftCapacity)
                {
                    A[k - 1] = Left[leftIndex++];
                }
                else
                {
                    A[k - 1] = Right[rightIndex++];
                }
            }
        }
        #endregion

        //TODO: Change this function to be MULTITHREADED
        //HINT: Remember to handle any dependency and/or critical section issues
        
        #region Multithreaded Sort [REMAINING TASKS]
        static int NumMergeSortThreads;

        #region Semaphores
        //TODO: Define any required semaphore here
        static Semaphore sortSemaphore1;   // Semaphore for sort thread 1
        static Semaphore sortSemaphore2;   // Semaphore for sort thread 2
        static Semaphore mergeSemaphore;   // Semaphore for merge thread
        #endregion

        #region Threads
        //TODO: Define any required thread objects here
        static Thread sortThread1;   // Sort thread 1
        static Thread sortThread2;   // Sort thread 2
        static Thread mergeThread;   // Merge thread
        #endregion

        #region Sort Function
        public static void SortMT(int[] array)
        {
            int s = 1;
            int e = array.Length;
            int m = (s + e) / 2;
            int node_idx = 0;

            NumMergeSortThreads = 2;                //TASK 2
            //NumMergeSortThreads = 4;              //TASK 3

            #region [TASK 2]
            if (NumMergeSortThreads == 2)       //TASK 2
            {
                /*TODO: 
                 * 1) Initialize any required semaphore
                 * 2) Create & Start TWO Merge Sort concurrent threads & ONE Merge thread
                 * 3) Use semaphores to handle any dependency or critical section
                 */
                // Initialize semaphores
                sortSemaphore1 = new Semaphore(0);
                sortSemaphore2 = new Semaphore(0);
                mergeSemaphore = new Semaphore(0);

                // Create and start the sorting threads
                sortThread1 = new Thread(new ParameterizedThreadStart(MSortMT));
                sortThread1.Start(Params2Object(array, s, m, m, node_idx));

                sortThread2 = new Thread(new ParameterizedThreadStart(MSortMT));
                sortThread2.Start(Params2Object(array, m + 1, e, m, node_idx));

                // Create and start the merge thread
                mergeThread = new Thread(new ParameterizedThreadStart(MergeMT));
                mergeThread.Start(Params2Object(array, s, e, m, node_idx));

                // Wait for both sorting threads to finish
                sortSemaphore1.Wait();
                sortSemaphore2.Wait();

                // Signal merge thread to proceed with merging
                mergeSemaphore.Signal();

                // Wait for the merge thread to finish
                mergeThread.Join();

            }
            #endregion

            #region [TASK 3]
            else if (NumMergeSortThreads == 4)   //TASK 3
            {
                /*TODO: 
                 * 1) Initialize any required semaphore
                 * 2) Create & Start TWO Merge Sort concurrent threads & ONE Merge thread
                 * 3) Use semaphores to handle any dependency or critical section
                 */
                // Initialize semaphores
                sortSemaphore1 = new Semaphore(0);
                sortSemaphore2 = new Semaphore(0);
                sortSemaphore3 = new Semaphore(0);
                sortSemaphore4 = new Semaphore(0);
                mergeSemaphore1 = new Semaphore(0);
                mergeSemaphore2 = new Semaphore(0);

                // Create and start the sorting threads
                sortThread1 = new Thread(new ParameterizedThreadStart(MSortMT));
                sortThread1.Start(Params2Object(array, s, m / 2, m / 2, node_idx));

                sortThread2 = new Thread(new ParameterizedThreadStart(MSortMT));
                sortThread2.Start(Params2Object(array, m / 2 + 1, m, m / 2, node_idx));

                sortThread3 = new Thread(new ParameterizedThreadStart(MSortMT));
                sortThread3.Start(Params2Object(array, m + 1, m + (m / 2), m / 2, node_idx));

                sortThread4 = new Thread(new ParameterizedThreadStart(MSortMT));
                sortThread4.Start(Params2Object(array, m + (m / 2) + 1, e, m / 2, node_idx));

                // Wait for all sorting threads to finish
                sortSemaphore1.Wait();
                sortSemaphore2.Wait();
                sortSemaphore3.Wait();
                sortSemaphore4.Wait();

                // Signal merge threads to proceed with merging
                mergeSemaphore1.Signal();
                mergeSemaphore2.Signal();

                // Wait for both merge threads to finish
                mergeThread1.Join();
                mergeThread2.Join();

                // Merge the results of the two merge threads
                mergeThread = new Thread(new ParameterizedThreadStart(MergeMT));
                mergeThread.Start(Params2Object(array, s, e, m, node_idx));

                // Wait for the final merge to finish
                mergeThread.Join();
            }

            #endregion
            
        }

        private static void MSortMT(object parameters)
        {
            #region Extract params from the given object 
            int[] A = null;
            int s = 0;
            int e = 0;
            int m = 0;
            int node_idx = 0;
            Object2Params(parameters, ref A, ref s, ref e, ref m, ref node_idx);
            #endregion

            
            MSort(A, s, e);

            #region [TASK 2] 
            if (NumMergeSortThreads == 2)       //TASK 2
            {
                //TODO: Use semaphores to handle any dependency or critical section
                // Signal the completion of sorting
                if (node_idx == 1)
                    sortSemaphore1.Signal();
                else if (node_idx == 2)
                    sortSemaphore2.Signal();
            }
            #endregion

            #region [TASK 3]
            else if (NumMergeSortThreads == 4)   //TASK 3
            {
                //TODO: Use semaphores to handle any dependency or critical section
            }
            #endregion            
        }

        private static void MergeMT(object parameters)
        {
            #region Extract params from the given object
            int[] A = null;
            int s = 0;
            int e = 0;
            int m = 0;
            int node_idx = 0;
            Object2Params(parameters, ref A, ref s, ref e, ref m, ref node_idx);
            #endregion
           
            #region [TASK 2]
            if (NumMergeSortThreads == 2)       //TASK 2
            {
                //TODO: Use semaphores to handle any dependency or critical section
                mergeSemaphore.Wait();
                Merge(A, s, m, e);
            }
            #endregion

            #region [TASK 3]
            else if (NumMergeSortThreads == 4)   //TASK 3
            {
                //TODO: Use semaphores to handle any dependency or critical section
                Merge(A, s, m, e);
            }
                
            #endregion
        }
        #endregion

        #endregion

    }
}
