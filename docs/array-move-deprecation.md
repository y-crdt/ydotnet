# Deprecation of `Array.Move`

`Array.Move` is deprecated. The native library removed the move feature in yrs 0.27, so the next version of YDotNet,
which uses a newer yrs, cannot offer it. This version still uses yrs 0.26 and `Array.Move` keeps working. Use this
version to prepare your data before you upgrade.

## Why this matters

Documents and updates that contain a move **cannot be loaded by yrs 0.27 or later**. Decoding fails for the whole
update, including the ordinary changes stored next to the move. `Transaction.ApplyV1` and `ApplyV2` report the failure
and leave the document unchanged. Code that ignores that result sees an empty document.

## What to do

1. **Stop calling `Array.Move`.** The compiler warns about every call. To move an element, remove it from its old
   position and insert it at the new one. This has two consequences:
   - The element gets a new identity. Other peers that edit the old element at the same time lose those edits if it is a
     nested type.
   - Concurrent moves of the same element do not converge. They can result in duplicates.
2. **Migrate stored documents that may contain moves**, while you are still on this version:
   1. Load each document.
   2. Read its content and write it into a new document, so that the result contains no moves.
   3. Encode the new document and store it in place of the old one.
3. **Check the result of applying persisted data** with `Transaction.ApplyV1` and `ApplyV2`, so that a failure is not
   mistaken for an empty document.

Once all stored data is free of moves, you can upgrade to the version that uses a newer yrs.
