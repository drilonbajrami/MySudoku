using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuGridLayout : MonoBehaviour
{

}

/* Dimensions Template - 950 x 950 total grid dimensions
950 - 2 x 10 (ob) outter borders
	- 2 x 6 (ib) inner borders
	- 6 x 3 (sb) slim borders
	- 9 x 300 cell size (cs)

w - 950
ob - ratio to width = (1/95 * w)
ib - ratio to width = (3/475 * w)
sb - ratio to width = (3/950 * w)
cs - ratio to width = (6/19 * w)

Row & Col spacing:
	x0 = ob + 1/2 * cs
	x1 = ob + sb + 3/2 * cs
	x2 = ob + 2sb + 5/2 * cs

	x3 = ob + 2sb + ib + 7/2 * cs
	x4 = ob + 3sb + ib + 9/2 * cs
	x5 = ob + 4sb + ib + 11/2 * cs

	x6 = ob + 4sb + 2ib + 13/2 * cs
	x7 = ob + 5sb + 2ib + 15/2 * cs
	x8 = ob + 6sb + 2ib + 17/2 * cs
 */