namespace Neo.Bpms.Domain.Models.WorkManagement;

public enum eAllocationOrOfferingAlgorithm
{
    Rotational = 1,
    Randomly = 2,
    FirstUser = 3,//Alwayes Allocate one user

    LastUserWhoSuccessfullyPerformedTheWork = 4,//successfully means compelmete the task and needs a formula field
    WithLeastAllocationToWork = 5,
    WithHighestUnemployment = 6,
    WithSmallestQueue = 7,
    WithHighestRank = 8
}