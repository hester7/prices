import gql from "graphql-tag";

export default gql`
    query enumMetadataQuery {
        priceIndexes {
            id
            name
            regionalTransmissionOperatorId
            priceMarketId
        }
        priceMarkets {
            id
            name
            abbreviation
        }
    }
`;
