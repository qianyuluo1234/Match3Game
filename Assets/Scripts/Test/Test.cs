using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Match3Game.Core;
using Match3Game.Core.EliminationBuilder;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Test
{
    public class Test : MonoBehaviour
    {
        [SerializeField]
        private Text _text;

        private Stopwatch _stopwatch1;
        private Stopwatch _stopwatch2;
        
        private CancellationTokenSource _preCheckCancellationTokenSource;
        private CancellationToken _preCheckCancellationToken;
        private void Awake()
        {
            
        }

        private void Start()
        {
            Match3Data match3Data = new Match3Data(10, 11, new[] {1, 2, 3, 4, 5, 6, 7});
            NormalElementDetector elementDetector = new NormalElementDetector();
            for (int i = 0; i < match3Data.types.Length; i++)
            {
                Match3Utility.BindElementEliminateBuilder(match3Data.types[i], elementDetector);
            }

            Match3Utility.CreateMap(match3Data);
            ShowMap(match3Data);
            
            GetAllEliminationBlocksAsync(match3Data);

            GetAllPreCheckBlocksAsync(match3Data);
        }

        private void ShowMap(Match3Data match3Data)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < match3Data.row; i++)
            {
                for (int j = 0; j < match3Data.column; j++)
                {
                    builder.Append(match3Data.map[i,j]).Append(" , ");
                }

                builder.Remove(builder.Length - 3, 3);
                builder.Append('\n');
            }

            _text.text = builder.ToString();
        }

        private async Task GetAllEliminationBlocksAsync(Match3Data match3Data)
        {
            Debug.Log("thread id:" + Thread.CurrentThread.ManagedThreadId);
            _stopwatch1 = Stopwatch.StartNew();
            List<EliminationBlock> blockList =
                await Task.Run(() => Match3Utility.FindAllEliminationBlocks(match3Data.map));
            
            Debug.Log("stopwatch: " + _stopwatch1.ElapsedMilliseconds);
            _stopwatch1.Stop();
        }
        
        private async Task GetAllPreCheckBlocksAsync(Match3Data match3Data)
        {
            _stopwatch2 = Stopwatch.StartNew();
            _preCheckCancellationTokenSource = new CancellationTokenSource();
            _preCheckCancellationToken = _preCheckCancellationTokenSource.Token;
            List<PreCheckBlock> blocksList =
                await Task.Run(() =>
                {
                    List<PreCheckBlock> temp = null;
                    try
                    {
                        if (!_preCheckCancellationTokenSource.IsCancellationRequested)
                        {
                            temp = Match3Utility.FindAllPreCheckBlocks(match3Data.map);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        throw;
                    }
                    finally
                    {
                        _preCheckCancellationTokenSource.Dispose();
                    }

                    return temp;
                }, _preCheckCancellationToken);
            Debug.Log("stopwatch PreCheck: " + _stopwatch2.ElapsedMilliseconds);
            _stopwatch2.Stop();
            
            if (blocksList == null)
            {
                return;
            }
            foreach (var block in blocksList)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("start: ").Append(block.startPos).Append(" target: ").Append(block.targetPos)
                    .Append('\n');
                for (int i = 0; i < block.count; i++)
                {
                    builder.Append(block[i]).Append(',');
                }
                builder.Remove(builder.Length - 1, 1);
                Debug.Log(builder.ToString());
            }
        }
    }
}