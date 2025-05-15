const SPEC_URL = 'http://localhost:5000/swagger/v1/swagger.json';

module.exports = {
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
          name: 'customAxiosMutator',
        },
        operations: {
          useTags: true,
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
};
