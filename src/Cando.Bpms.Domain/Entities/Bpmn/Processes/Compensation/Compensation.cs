//Compensation is concerned with undoing steps that were already successfully completed, because their results and
//possibly side effects are no longer desired and need to be reversed. If an Activity is still active, it cannot be compensated,
//but rather needs to be canceled. Cancellation in turn can result in compensation of already successfully completed
//portions of an active Activity, in case of a Sub-Process.
//Compensation is performed by a compensation handler. A compensation handler performs the steps necessary to reverse
//the effects of an Activity. In case of a Sub-Process, the compensation handler has access to Sub-Process data at the
//time of its completion (“snapshot data”).
//Compensation is triggered by a throw Compensation Event, which typically will be raised by an error handler, as part
//of cancellation, or recursively by another compensation handler. That Event specifies the Activity for which
//compensation is to be performed, either explicitly or implicitly.

