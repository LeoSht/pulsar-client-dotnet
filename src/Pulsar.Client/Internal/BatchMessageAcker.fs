﻿namespace Pulsar.Client.Internal

open System.Collections
open FSharp.UMX
open Pulsar.Client.Common

type BatchMessageAcker internal (batchSize: int) =
    let bitSet = BitArray(batchSize, true)
    let mutable unackedCount = batchSize

    member internal this.AckIndividual (batchIndex: BatchIndex) =
        let previous = bitSet.[%batchIndex]
        if previous then
            bitSet.Set(%batchIndex, false)
            unackedCount <- unackedCount - 1
        unackedCount = 0

    member internal this.AckGroup (batchIndex: BatchIndex) =
        for i in 0 .. %batchIndex do
            if bitSet.[i] then
                bitSet.[i] <- false
                unackedCount <- unackedCount - 1
        unackedCount = 0
    
    member internal this.BitSet = bitSet
    
    // debug purpose
    member internal this.GetOutstandingAcks() =
        unackedCount

    member internal this.GetBatchSize() =
        batchSize

    member val PrevBatchCumulativelyAcked = false with get, set

    // Stub for batches that don't need acker at all
    static member NullAcker =
        Unchecked.defaultof<BatchMessageAcker>

    override this.Equals _ = true

    override this.GetHashCode () = 0

    override this.ToString() = "UnackedCount: " + unackedCount.ToString()

    interface System.IComparable with
         member x.CompareTo _ = 0
    interface System.IComparable<BatchMessageAcker> with
         member x.CompareTo _ = 0