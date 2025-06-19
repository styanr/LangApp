import { defineConfig } from 'orval';

const SPEC_URL = 'http://localhost:5000/swagger/v1/swagger.json';
const FUNCTIONS_SPEC_URL = 'http://localhost:7158/api/swagger.json';

export default defineConfig({
  langapp: {
    input: {
      target: SPEC_URL,
    },
    output: {
      mode: 'tags',
      target: './api/orval',
      client: 'react-query',
      override: {
        mutator: {
          path: './api/axiosMutator.ts',
          name: 'mainApiMutator',
        },
        query: {
          useQuery: true,
          useInfinite: true,
          useInfiniteQueryParam: 'pageNumber',
          options: {
            staleTime: 10000,
          },
        },
      },
      prettier: true,
    },
  },
  'langapp-functions': {
    input: {
      target: FUNCTIONS_SPEC_URL,
    },
    output: {
      mode: 'tags',
      target: './api/functions/orval',
      client: 'react-query',
      override: {
        mutator: {
          path: './api/axiosMutator.ts',
          name: 'functionsApiMutator',
        },
        query: {
          useQuery: true,
          options: {
            staleTime: 5 * 1000 * 60, // 25 minutes
          },
        },
      },
      prettier: true,
    },
  },
});
