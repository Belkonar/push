/**
 * function to make sure elements of a combineLatest
 * are filled out.
 * @param data array to be checked
 */
export function filled(data: any[]): boolean {
  for (const val of data) {
    if (val === null) {
      return false;
    }
  }

  return true;
}
