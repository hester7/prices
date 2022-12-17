import gql from "graphql-tag";

export default gql`
    subscription onCurrentPriceChange {
        onCurrentPriceChange {
            id
            name
            regionalTransmissionOperatorId
            currentPrice
            change24Hour
            lastModifiedAtUtc
        }
    }
`;
