import { useMemo } from 'react';

/**
 * Result of parsing fill-in-the-blank template text
 */
export interface BlankParserResult {
  /** Parsed segments of text and blanks */
  parsedParts: Array<{ type: 'text'; content: string } | { type: 'blank'; localIndex: number }>;
  /** Total number of blanks detected */
  blanksCount: number;
}

/**
 * Hook to parse fill-in-the-blank template text
 * Detects blanks (underscores) and splits text into segments
 *
 * @param templateText - The template text to parse for blanks
 * @returns BlankParserResult containing parsed parts and blank count
 */
export function useFillInTheBlankParser(templateText: string | undefined): BlankParserResult {
  return useMemo(() => {
    const parts: Array<{ type: 'text'; content: string } | { type: 'blank'; localIndex: number }> =
      [];
    if (!templateText) return { parsedParts: parts, blanksCount: 0 };

    // Enhanced regex that handles both simple and advanced cases:
    // 1. Underscore surrounded by spaces
    // 2. Underscore at beginning/end of text
    // 3. Underscore next to punctuation
    // 4. Multiple consecutive underscores treated as a single blank
    // This pattern is fully compatible with both the activity and form components
    const jsRegex = /(?:^|\s)(_+)(?=\s|$|[.,;!?)]|\p{P})/gu;
    let lastIndex = 0;
    let blankLocalIndex = 0;
    let match;

    while ((match = jsRegex.exec(templateText)) !== null) {
      const fullMatch = match[0];
      const underscore = match[1];
      const matchStart = match.index;
      const underscoreStartInFullMatch = fullMatch.indexOf(underscore);
      const underscoreAbsoluteStart = matchStart + underscoreStartInFullMatch;

      if (underscoreAbsoluteStart > lastIndex) {
        parts.push({
          type: 'text',
          content: templateText.substring(lastIndex, underscoreAbsoluteStart),
        });
      }
      parts.push({ type: 'blank', localIndex: blankLocalIndex++ });
      lastIndex = underscoreAbsoluteStart + underscore.length;
    }

    if (lastIndex < templateText.length) {
      parts.push({ type: 'text', content: templateText.substring(lastIndex) });
    }

    return { parsedParts: parts, blanksCount: blankLocalIndex };
  }, [templateText]);
}
